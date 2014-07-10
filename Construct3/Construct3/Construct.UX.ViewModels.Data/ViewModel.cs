using System;
using System.Linq;
using Construct.Utilities.Shared;
using System.ServiceModel;
using Construct.UX.ViewModels.Data.DataServiceReference;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace Construct.UX.ViewModels.Data
{
    public class ViewModel : ViewModels.ViewModel
    {
        private ModelClient model = null;
        private CallbackImplementation callback = new CallbackImplementation();
        private InstanceContext instanceContext = null;

        public ObservableCollection<DataType> ObservableDataTypes { get; private set; }

        public ViewModel(ApplicationSessionInfo applicationSessionInfo)
            : base(applicationSessionInfo, "Data")
        {
            model = GetModel();
            ObservableDataTypes = new ObservableCollection<DataType>(GetAllTypes());
        }

        private ModelClient GetModel()
        {
            instanceContext = new InstanceContext(callback);
            ModelClient client = new ModelClient(instanceContext, "duplexendpoint", RemoteAddress);
            client.Open();
            return client;
        }

		public SpecificTimeSpan? GetTimeSpanForTypeAndSource(Guid dataTypeId, Guid sourceId)
		{
			DateTime? startTime = model.GetEarliestItemForTypeAndSource(dataTypeId, sourceId);
			if (!startTime.HasValue)
				return null;

			DateTime? endTime = model.GetLatestItemForTypeAndSource(dataTypeId, sourceId);
			if (!endTime.HasValue)
				return null;

			return new SpecificTimeSpan(startTime.Value, endTime.Value);
		}

        public override void Load()
        {
        }

        public bool AddTypeWithXML(string xml)
        {
            return model.AddTypeWithXML(xml);
        }
        public bool AddTypeWithDataType(DataServiceReference.DataTypeSource dataTypeSource, DataServiceReference.DataType dataType, IEnumerable<DataServiceReference.PropertyType> propertyTypes)
        {
            return model.AddTypeWithDataType(dataTypeSource, dataType, propertyTypes.ToArray(), true);
        }

        public bool SetContext(string connectionString)
        {
            return model.SetContext(connectionString);
        }

        public void Add(DataServiceReference.Datum datum)
        {
            model.Add(datum);
        }

        public DataServiceReference.DataType[] GetAllTypes()
        {
            DataType[] dataTypes = model.GetAllTypes();
            ObservableDataTypes = new ObservableCollection<DataType>(dataTypes);
            return dataTypes;
        }

		int numModelRequests = 0;
		int requestsAvoided = 0;

		private TimeSpan alignmentSpan = TimeSpan.FromMinutes(30);
		private Dictionary<Guid, List<ItemFrequencyData>> alignedItemFrequencies = new Dictionary<Guid, List<ItemFrequencyData>>();

		private uint[] GenerateFrequenciesFromAlignedCache(Guid sourceId, TimeSpan interval, DateTime startTime, DateTime endTime)
		{
			uint numRequestedFrequencies = (uint)((endTime.Ticks - startTime.Ticks) / interval.Ticks);
			uint[] result = new uint[numRequestedFrequencies];

			var alignedSourceItemFrequencies = alignedItemFrequencies[sourceId];

			for (uint i = 0; i < numRequestedFrequencies; i++)
			{
				SpecificTimeSpan currentFrame = new SpecificTimeSpan(startTime + TimeSpan.FromTicks(interval.Ticks * i), interval);
				var overlappingCacheEntries = alignedSourceItemFrequencies.Where(entry => entry.SpecificTimeSpan.GetOverlap(currentFrame) != TimeSpan.Zero);

				uint currentFrequency = 0;

				foreach (var relevantEntry in overlappingCacheEntries)
				{
					double weighting = relevantEntry.SpecificTimeSpan.GetOverlap(currentFrame).TotalMinutes / interval.TotalMinutes;
					uint weightedFrequency = (uint)Math.Round(relevantEntry.ItemFrequency * weighting);
					currentFrequency += weightedFrequency;
				}

				result[i] = currentFrequency;
			}

			return result;
		}

		private void FillAlignedFrequencyCache(Guid sourceId, DateTime startTime, DateTime endTime)
		{
			List<ItemFrequencyData> largeChunks = new List<ItemFrequencyData>();

			startTime = new DateTime(startTime.Ticks);
			endTime = new DateTime(endTime.Ticks);

			startTime -= TimeSpan.FromTicks(startTime.Ticks % alignmentSpan.Ticks);
			if (endTime.Ticks % alignmentSpan.Ticks != 0)
				endTime += TimeSpan.FromTicks(alignmentSpan.Ticks - (endTime.Ticks % alignmentSpan.Ticks));

			List<ItemFrequencyData> alignedSourceItemFrequencies;
			if (alignedItemFrequencies.ContainsKey(sourceId))
			{
				alignedSourceItemFrequencies = alignedItemFrequencies[sourceId];
			}
			else
			{
				alignedSourceItemFrequencies = new List<ItemFrequencyData>();
				alignedItemFrequencies.Add(sourceId, alignedSourceItemFrequencies);
			}

			if (alignedSourceItemFrequencies.Count == 0)
			{
				largeChunks.Add(new ItemFrequencyData(model.GetNumberOfItemsInTimespan(startTime, endTime, null, sourceId), new SpecificTimeSpan(startTime, endTime)));
			}
			else
			{
				DateTime earliestSegment = alignedSourceItemFrequencies.Select(itemFrequency => itemFrequency.SpecificTimeSpan.Start).Min();
				//	If they requested data that's earlier than what's cached
				if ((earliestSegment - startTime).Ticks > 0)
				{
					SpecificTimeSpan leftTimeSpan = new SpecificTimeSpan();
					leftTimeSpan.Start = startTime;
					if ((endTime - earliestSegment).Ticks > 0)
						leftTimeSpan.End = earliestSegment;
					else
						leftTimeSpan.End = endTime;
					largeChunks.Add(new ItemFrequencyData(model.GetNumberOfItemsInTimespan(leftTimeSpan.Start, leftTimeSpan.End, null, sourceId), leftTimeSpan));
				}

				//	If they requested data that's later than what's cached
				DateTime latestSegment = alignedSourceItemFrequencies.Select(itemFrequency => itemFrequency.SpecificTimeSpan.End).Max();
				if ((endTime - latestSegment).Ticks > 0)
				{
					SpecificTimeSpan rightTimeSpan = new SpecificTimeSpan();
					rightTimeSpan.End = endTime;
					if ((latestSegment - startTime).Ticks > 0)
						rightTimeSpan.Start = latestSegment;
					else
						rightTimeSpan.Start = startTime;
					largeChunks.Add(new ItemFrequencyData(model.GetNumberOfItemsInTimespan(rightTimeSpan.Start, rightTimeSpan.End, null, sourceId), rightTimeSpan));
				}
			}

			//	Run through our large chunks until they've all been broken into interval chunks
			while (largeChunks.Count > 0)
			{
				var currentChunk = largeChunks[largeChunks.Count - 1];
				largeChunks.RemoveAt(largeChunks.Count - 1);


				int totalIntervalsInChunk = (int)(currentChunk.SpecificTimeSpan.TimeSpan.Ticks / alignmentSpan.Ticks);

				//	If this chunk is an entire interval then it doesn't need to be split anymore
				if (totalIntervalsInChunk == 1)
				{
					alignedSourceItemFrequencies.Add(currentChunk);
					continue;
				}

				if (currentChunk.ItemFrequency == 0)
				{
					//	There are no items in this chunk, make enough empty sub-chunks to cover this empty chunk and then throw it away
					ItemFrequencyData currentFillerIntervalChunk;
					SpecificTimeSpan currentChunkTime = new SpecificTimeSpan(currentChunk.SpecificTimeSpan.Start, alignmentSpan);
					for (int s = 0; s < totalIntervalsInChunk; s++)
					{
						currentFillerIntervalChunk = new ItemFrequencyData();
						currentFillerIntervalChunk.ItemFrequency = 0;
						currentFillerIntervalChunk.SpecificTimeSpan = currentChunkTime;
						alignedSourceItemFrequencies.Add(currentFillerIntervalChunk);

						currentChunkTime += alignmentSpan;
					}

					requestsAvoided += totalIntervalsInChunk;

					continue;
				}

				//	Split up the chunk into left/right chunks, each approx. half the current chunk. 
				SpecificTimeSpan leftChunkSpan, rightChunkSpan;
				int intervalsInLeft = totalIntervalsInChunk / 2;

				leftChunkSpan = new SpecificTimeSpan(currentChunk.SpecificTimeSpan.Start, TimeSpan.FromTicks(intervalsInLeft * alignmentSpan.Ticks));
				rightChunkSpan = new SpecificTimeSpan(leftChunkSpan.End, currentChunk.SpecificTimeSpan.End);

				ItemFrequencyData leftFrequencyData = new ItemFrequencyData();
				leftFrequencyData.ItemFrequency = model.GetNumberOfItemsInTimespan(leftChunkSpan.Start, leftChunkSpan.End, null, sourceId);
				leftFrequencyData.SpecificTimeSpan = leftChunkSpan;
				numModelRequests++;

				ItemFrequencyData rightFrequencyData = new ItemFrequencyData();
				//	The right chunk contains the items not in the left, but in currentChunk, therefore its number of items must be (total - left), avoid a query
				rightFrequencyData.ItemFrequency = currentChunk.ItemFrequency - leftFrequencyData.ItemFrequency;
				rightFrequencyData.SpecificTimeSpan = rightChunkSpan;
				requestsAvoided++;

				largeChunks.Add(leftFrequencyData);
				largeChunks.Add(rightFrequencyData);
			}
		}

		public uint[] GetItemFrequencies(Guid[] sources, TimeSpan interval, DateTime startTime, DateTime endTime)
		{
			uint numRequestedFrequencies = (uint)((endTime.Ticks - startTime.Ticks) / interval.Ticks);
			uint[] result = new uint[numRequestedFrequencies];
			List<uint[]> sourceFrequencies = new List<uint[]>();

			foreach (Guid source in sources)
				sourceFrequencies.Add(GetItemFrequencies(source, interval, startTime, endTime));

			for (int i = 0; i < sourceFrequencies.Count; i++)
			{
				for (int j = 0; j < result.Length; j++)
					result[j] += sourceFrequencies[i][j];
			}

			return result;
		}

		public uint[] GetItemFrequencies(Guid source, TimeSpan interval, DateTime startTime, DateTime endTime)
		{
			startTime = startTime.ToUniversalTime();
			endTime = endTime.ToUniversalTime();

			FillAlignedFrequencyCache(source, startTime, endTime);
			return GenerateFrequenciesFromAlignedCache(source, interval, startTime, endTime);
		}

		public List<String> GetDataLabels(Guid[] dataTypeIds)
		{
			Guid[] header = model.GenerateConstructHeaders(dataTypeIds);
			return model.GenerateConstructHeaderNames(header).ToList();
		}

		public object[][] GetData(Guid[] targetDataTypes, Guid[] sourceSensors, DateTime startTime, DateTime endTime, TimeSpan sampleInterval)
		{
			Guid[] header = model.GenerateConstructHeaders(targetDataTypes);

			return model.GenerateConstruct(startTime, endTime, sampleInterval, header, sourceSensors);
		}

        /* todo 10/11
        public ReadOnlyCollection<Uri> GetUri(DataServiceReference.DataType dataType, DataServiceReference.PropertyType propertyType)
        {
            return model.GetUris(dataType, propertyType);
        }
         */
    }
}