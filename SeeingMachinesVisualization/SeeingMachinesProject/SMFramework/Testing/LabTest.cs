using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ServiceStack.Text;

/*
 * A LabTest contains multiple LabTestStages. A LabTestStage can contain multiple LabTestProcedures. A LabTestProcedure defines
 *	the methods of data analysis that are applied for the data captured doing that LabTestStage.
 * 
 * i.e. There is a LabTestStage that prompts the user for: "Have the test subjects stare at point (x, y, z)." There would be
 *			a corresponding LabTestProcedure for recording Vergence data, and possibly another LabTestProcedure for recording
 *			head orientation/positioning data. 
 * 
 * The LabTestStage provides meaningful details of the test and its duration, while the LabTestProcedures define the data
 *			analysis for the duration of that stage.
 * 
 * 
 * TODO: Should LabTest also provide a sort of TestsAnalysis object to allow C# code to work with the analytics results? Right
 *			now the only option is to export to a specified format.
 * 
 */

namespace SMFramework.Testing
{
	public class TestsAnalysis
	{
		#region ServiceStack JSON Accommodation

		/*
		 * These functions define the serialization procedure for ProcedureOutput objects, which are not directly
		 *		supported by ServiceStack's JSON parsing implementation. As a result, we need this manual
		 *		override for ProcedureOutput serialization.
		 */
		static TestsAnalysis()
		{
			JsConfig<ProcedureOutput>.RawSerializeFn = SerializeProcedureResults;
		}

		internal static String SerializeProcedureResults(ProcedureOutput output)
		{
			return output.ToFormat(LabTest.ReportFormat.JSON);
		}
		#endregion

		public SensorClusterConfiguration SourceSensors
		{
			get;
			set;
		}

		public List<String> Categories
		{
			get
			{
				List<String> result = new List<String>();

				foreach (TestStage.Output output in AnalysisOutput)
				{
					foreach (ProcedureOutput procedure in output.ProcedureResults)
					{
						if (!result.Contains(procedure.Category))
							result.Add(procedure.Category);
					}
				}

				return result;
			}
		}

		public List<TestStage.Output> AnalysisOutput
		{
			get;
			set;
		}

		public String ToFormat(LabTest.ReportFormat format)
		{
			switch (format)
			{
				case LabTest.ReportFormat.JSON:
					return JsonSerializer.SerializeToString(this);

				case LabTest.ReportFormat.JSONP:
					return "TestAnalysisLoad(" + this.ToFormat(LabTest.ReportFormat.JSON) + ");";

				default:
					return "Invalid report format.";
			}
		}

		internal TestsAnalysis()
		{
			AnalysisOutput = new List<TestStage.Output>();
		}
	}

	public class LabTest
	{
		public List<TestStage> Stages
		{
			get;
			private set;
		}

		public TestsAnalysis Results
		{
			get;
			private set;
		}

		public SensorClusterConfiguration Sensors;
		private int m_CurrentStageIndex = -1;

		public LabTest(SensorClusterConfiguration sourceSensors)
		{
			Stages = new List<TestStage>();
			Results = new TestsAnalysis();
			Sensors = sourceSensors;
		}

		/// <summary>
		/// Creates a new stage and adds the stage to the end of the list of stages in the test.
		/// </summary>
		/// <returns>The new stage that was created.</returns>
		public TestStage CreateStage(String stageName, DataCollectionSource dataSource)
		{
			TestStage stage = new TestStage(stageName, dataSource);
			Stages.Add(stage);
			stage.SourceSensors = Sensors;

			return stage;
		}

		public enum ReportFormat
		{
			JSON,
			JSONP
		}

		public TestStage CurrentStage
		{
			get
			{
				if (m_CurrentStageIndex < 0 || Stages.Count == 0)
					return null;
				else
					return Stages[Math.Min(m_CurrentStageIndex, Stages.Count - 1)];
			}
		}

		public int CurrentStageIndex
		{
			get { return m_CurrentStageIndex; }
		}

		/// <summary>
		/// Advances the current stage and begins data capturing for that stage.
		/// </summary>
		/// <returns>True when there are stages left after advancing, false when there are no more stages to run.</returns>
		public bool AdvanceCurrentStage()
		{
			if (m_CurrentStageIndex >= 0 && !Stages[m_CurrentStageIndex].IsDone)
			{
				DebugOutputStream.SlowInstance.WriteLine(
					"LabTest.RunNextStage - Unable to start new stage when the previous stage has not yet finished."
					);

				return true;
			}

			m_CurrentStageIndex++;

			/*
			 * The stage should be advanced for the user, but not yet started. Starting will be done by the
			 *	user via CurrentStage.BeginCapture().
			 */

			return m_CurrentStageIndex < Stages.Count;
		}

		public void RegressCurrentStage()
		{
			if (m_CurrentStageIndex >= 0)
				m_CurrentStageIndex--;
		}

		public TestsAnalysis GenerateTestReport()
		{
			TestsAnalysis result = new TestsAnalysis();
			result.SourceSensors = Sensors;

			if (Stages.Count == 0)
				return result;

			if (!CurrentStage.IsDone)
			{
				//	... what SHOULD we do here?
			}

			foreach (TestStage stage in Stages)
			{
				result.AnalysisOutput.Add(stage.GenerateProcedureResults());
			}

			return result;
		}

		public String GenerateTestReport(ReportFormat format)
		{
			return GenerateTestReport().ToFormat(format);
			//DebugOutputStream.SlowInstance.WriteLine("LabTest.GenerateTestReport was provided an invalid ReportFormat: " + format.ToString());
		}

		public void GenerateTestReport(String fileTarget, ReportFormat format)
		{
			using (StreamWriter writer = new StreamWriter(fileTarget, false))
			{
				writer.Write(GenerateTestReport(format));
			}
		}

		public String SaveStageDataToDisk()
		{
			if (!Directory.Exists("Data"))
				Directory.CreateDirectory("Data");

			String folderName = DateTime.Now.ToString("MMM-dd-yyyy   hh-mm-ss-tt");

			SaveStageDataToDisk("Data/" + folderName);

			return folderName;
		}

		public void SaveStageDataToDisk(String folderName)
		{
			Directory.CreateDirectory(folderName);

			foreach (TestStage stage in Stages)
			{
				// For stages that ended without saving data
				if (stage.ResultData == null)
					continue;

				if (Path.IsPathRooted(stage.DataSource.PersistenceFileName))
					stage.ResultData.WriteToDisk(stage.DataSource.PersistenceFileName);
				else
					stage.ResultData.WriteToDisk(folderName + "/" + stage.DataSource.PersistenceFileName + ".csv");
			}
		}
	}

	public class TestStage
	{
		public class Output
		{
			public Output()
			{
				ProcedureResults = new List<ProcedureOutput>();
			}

			public String StageName
			{
				get;
				set;
			}

			public List<ProcedureOutput> ProcedureResults
			{
				get;
				set;
			}
		}

		List<TestProcedure> m_Procedures = new List<TestProcedure>();

		public delegate void StageStartHandler(TestStage stage);
		public delegate void StageEndHandler(TestStage stage, TimeSpan duration);

		public event StageStartHandler StageStarting;
		public event StageEndHandler StageEnded;

		public SensorClusterConfiguration SourceSensors
		{
			get;
			internal set;
		}

		DateTime m_StartTime;
		private DataCollectionSource m_DataSource;
		public DataCollectionSource DataSource
		{
			get { return m_DataSource; }
			set
			{
				if (IsDone)
					m_DataSource = value;
				else
					DebugOutputStream.SlowInstance.WriteLine("Cannot assign a DataCollectionSource to a LabStage when collection for the stage has already begun.");
			}
		}

		internal TestStage()
		{
			IsDone = true;
		}

		public bool IsDone
		{
			get;
			private set;
		}

		public PairedDatabase ResultData
		{
			get;
			private set;
		}

		public String Name
		{
			get;
			private set;
		}

		public TestStage(String stageName, DataCollectionSource dataSource)
		{
			Name = stageName;
			m_DataSource = dataSource;
		}

		internal Output GenerateProcedureResults()
		{
			Output result = new Output();
			result.StageName = Name;

			foreach (TestProcedure procedure in m_Procedures)
			{
				result.ProcedureResults.Add(procedure.GetOutput(ResultData));
			}

			return result;
		}

		public void BeginCapture()
		{
			if (StageStarting != null)
				StageStarting(this);

			m_StartTime = DateTime.Now;
			IsDone = false;

			DataSource.CollectionCompleted += this.DataSource_CollectionCompleted;
			DataSource.BeginCollection(SourceSensors);
		}

		void DataSource_CollectionCompleted()
		{
			IsDone = true;
			ResultData = DataSource.CollectedData;

			if (StageEnded != null)
				StageEnded(this, DateTime.Now - m_StartTime);

			DataSource.CollectionCompleted -= this.DataSource_CollectionCompleted;
		}

		public void EndCapture()
		{
			DataSource.CollectionCompleted -= this.DataSource_CollectionCompleted;

			DataSource.StopCollection();

			IsDone = true;
			ResultData = DataSource.CollectedData;

			if (StageEnded != null)
				StageEnded(this, DateTime.Now - m_StartTime);
		}

		public void AddProcedure(TestProcedure procedure)
		{
			m_Procedures.Add(procedure);
		}
	}
}
