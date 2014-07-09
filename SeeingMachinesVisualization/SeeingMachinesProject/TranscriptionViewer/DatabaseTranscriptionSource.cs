using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TranscriptionViewer
{
	struct Transcription
	{
		public Guid ItemID;
		public String Text;
		public DateTime Timestamp;
		public Guid Source;
	}

	public class DatabaseTranscriptionSource : IDisposable
	{
		public delegate void OnNewTranscription(Guid sourceId, DateTime transcriptionTime, String transcription);

		public event OnNewTranscription NewTranscription
		{
			add
			{
				var handler = value;
				if (handler != null)
				{
					for (int i = 0; i < gatheredTranscriptions.Count; i++)
					{
						var transcription = gatheredTranscriptions[i];
						handler(transcription.Source, transcription.Timestamp, transcription.Text);
					}
				}

				newTranscription += handler;
			}
			remove
			{
				newTranscription += value;
			}
		}

		private event OnNewTranscription newTranscription;

		//	New event handlers are pushed all existing gathered transcriptions, previously detected transcriptions aren't reported twice
		List<Transcription> gatheredTranscriptions = new List<Transcription>();

		bool shouldContinue;
		String connectionString;
		Task transcriptionScanTask;

		public DatabaseTranscriptionSource(String connectionString)
		{
			this.connectionString = connectionString;

			this.shouldContinue = true;
			this.transcriptionScanTask = Task.Factory.StartNew(TranscriptionScanThread, DateTime.UtcNow - TimeSpan.FromHours(1));
		}

		public void Dispose()
		{
			shouldContinue = false;
			transcriptionScanTask.Wait();
		}

		void TranscriptionScanThread(object userData)
		{
			DateTime queryStartTime = (DateTime)userData;

			SqlConnection constructDatabaseConnection = new SqlConnection(connectionString);

			while (shouldContinue)
			{
				if (constructDatabaseConnection.State != System.Data.ConnectionState.Open)
					constructDatabaseConnection.Open();

				String transcriptionsDatabaseName = "z_Transcription_TranscribedText";
				String queryString = "SELECT [ItemID],[SourceID],[StartTime],[Value] FROM " + transcriptionsDatabaseName + " WHERE StartTime>@baseTime ORDER BY StartTime";

				SqlCommand queryCommand = constructDatabaseConnection.CreateCommand();
				queryCommand.CommandText = queryString;
				queryCommand.Parameters.Add(new SqlParameter("baseTime", System.Data.SqlDbType.DateTime2)).Value = queryStartTime;

				DateTime newStartTime = queryStartTime;

				SqlDataReader reader;
				try
				{
					reader = queryCommand.ExecuteReader();
				}
				catch (Exception e)
				{
					Thread.Sleep(1);
					continue;
				}

				while (reader.Read())
				{
					var dataRow = reader as IDataRecord;
					Transcription currentTranscription = new Transcription();
					currentTranscription.ItemID = dataRow.GetGuid(0);
					currentTranscription.Source = dataRow.GetGuid(1);
					currentTranscription.Timestamp = dataRow.GetDateTime(2);
					currentTranscription.Text = dataRow.GetString(3);

					if (!gatheredTranscriptions.Any(t => t.ItemID == currentTranscription.ItemID))
					{
						gatheredTranscriptions.Add(currentTranscription);

						if ((currentTranscription.Timestamp - queryStartTime).Ticks > 0)
							newStartTime = currentTranscription.Timestamp;

						if (newTranscription != null)
							newTranscription(currentTranscription.Source, currentTranscription.Timestamp, currentTranscription.Text);
					}
				}

				reader.Close();

				if (queryStartTime != newStartTime)
				{
					//	Start from 30 seconds before the latest, to allow us to capture transcriptions that may have come in later but where captured earlier
					newStartTime = queryStartTime - TimeSpan.FromSeconds(30);
				}

				Thread.Sleep(1);
			}
		}
	}
}
