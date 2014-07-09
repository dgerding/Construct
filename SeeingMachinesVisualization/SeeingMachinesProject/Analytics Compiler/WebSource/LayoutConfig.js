
ProcedureResponders = {
	DirectionToPointProcedure: function (dataObject, templateElement) {
		templateElement.getElementsByClassName("AverageAngle")[0].innerHTML = dataObject.AverageAngle.toString();
		templateElement.getElementsByClassName("MinAngle")[0].innerHTML = dataObject.MinAngle.toString();
		templateElement.getElementsByClassName("MaxAngle")[0].innerHTML = dataObject.MaxAngle.toString();
		templateElement.getElementsByClassName("StandardDeviation")[0].innerHTML = dataObject.StandardDeviation.toString();
		templateElement.getElementsByClassName("FirstQuartile")[0].innerHTML = dataObject.FirstQuartile.toString();
		templateElement.getElementsByClassName("SecondQuartile")[0].innerHTML = dataObject.SecondQuartile.toString();
		templateElement.getElementsByClassName("ThirdQuartile")[0].innerHTML = dataObject.ThirdQuartile.toString();

		var whiskerPlot = templateElement.getElementsByClassName("WhiskerPlot")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngle,
			dataObject.FirstQuartile,
			dataObject.SecondQuartile,
			dataObject.ThirdQuartile,
			dataObject.MaxAngle,
            dataObject.AverageAngle,
            dataObject.StandardDeviation
			));



		templateElement.getElementsByClassName("XYMean")[0].innerHTML = dataObject.MeanAngleDegreesXY;
		templateElement.getElementsByClassName("XYSD")[0].innerHTML = dataObject.StandardAngularDeviationDegreesXY;
		templateElement.getElementsByClassName("XYMin")[0].innerHTML = dataObject.MinAngleDegreesXY;
		templateElement.getElementsByClassName("XYQ1")[0].innerHTML = dataObject.FirstAngularDegreesXY;
		templateElement.getElementsByClassName("XYQ2")[0].innerHTML = dataObject.SecondAngularDegreesXY;
		templateElement.getElementsByClassName("XYQ3")[0].innerHTML = dataObject.ThirdAngularDegreesXY;
		templateElement.getElementsByClassName("XYMax")[0].innerHTML = dataObject.MaxAngleDegreesXY;

		whiskerPlot = templateElement.getElementsByClassName("XYWhisker")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegreesXY,
			dataObject.FirstAngularDegreesXY,
			dataObject.SecondAngularDegreesXY,
			dataObject.ThirdAngularDegreesXY,
			dataObject.MaxAngleDegreesXY,
			dataObject.MeanAngleDegreesXY,
			dataObject.StandardAngularDeviationDegreesXY
			));




		templateElement.getElementsByClassName("XZMean")[0].innerHTML = dataObject.MeanAngleDegreesXZ;
		templateElement.getElementsByClassName("XZSD")[0].innerHTML = dataObject.StandardAngularDeviationDegreesXZ;
		templateElement.getElementsByClassName("XZMin")[0].innerHTML = dataObject.MinAngleDegreesXZ;
		templateElement.getElementsByClassName("XZQ1")[0].innerHTML = dataObject.FirstAngularDegreesXZ;
		templateElement.getElementsByClassName("XZQ2")[0].innerHTML = dataObject.SecondAngularDegreesXZ;
		templateElement.getElementsByClassName("XZQ3")[0].innerHTML = dataObject.ThirdAngularDegreesXZ;
		templateElement.getElementsByClassName("XZMax")[0].innerHTML = dataObject.MaxAngleDegreesXZ;

		whiskerPlot = templateElement.getElementsByClassName("XZWhisker")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegreesXZ,
			dataObject.FirstAngularDegreesXZ,
			dataObject.SecondAngularDegreesXZ,
			dataObject.ThirdAngularDegreesXZ,
			dataObject.MaxAngleDegreesXZ,
			dataObject.MeanAngleDegreesXZ,
			dataObject.StandardAngularDeviationDegreesXZ
			));





		templateElement.getElementsByClassName("YZMean")[0].innerHTML = dataObject.MeanAngleDegreesYZ;
		templateElement.getElementsByClassName("YZSD")[0].innerHTML = dataObject.StandardAngularDeviationDegreesYZ;
		templateElement.getElementsByClassName("YZMin")[0].innerHTML = dataObject.MinAngleDegreesYZ;
		templateElement.getElementsByClassName("YZQ1")[0].innerHTML = dataObject.FirstAngularDegreesYZ;
		templateElement.getElementsByClassName("YZQ2")[0].innerHTML = dataObject.SecondAngularDegreesYZ;
		templateElement.getElementsByClassName("YZQ3")[0].innerHTML = dataObject.ThirdAngularDegreesYZ;
		templateElement.getElementsByClassName("YZMax")[0].innerHTML = dataObject.MaxAngleDegreesYZ;

		whiskerPlot = templateElement.getElementsByClassName("YZWhisker")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegreesYZ,
			dataObject.FirstAngularDegreesYZ,
			dataObject.SecondAngularDegreesYZ,
			dataObject.ThirdAngularDegreesYZ,
			dataObject.MaxAngleDegreesYZ,
			dataObject.MeanAngleDegreesYZ,
			dataObject.StandardAngularDeviationDegreesYZ
			));
	},

	AngleBetweenAngleProcedure: function (dataObject, templateElement) {

	},

	DistanceBetweenPointsProcedure: function (dataObject, templateElement) {

	},

	DataReliabilityProcedure: function (dataObject, templateElement) {
		templateElement.getElementsByClassName("Reliability")[0].innerHTML = dataObject.Reliability;
		templateElement.getElementsByClassName("NonNull")[0].innerHTML = dataObject.NonNullSnapshots;
		templateElement.getElementsByClassName("Null")[0].innerHTML = dataObject.NullSnapshots;
		templateElement.getElementsByClassName("Total")[0].innerHTML = dataObject.TotalSnapshotsCount;
	},

	GenericVectorProcedure: function (dataObject, templateElement) {
		templateElement.getElementsByClassName("AverageMagnitude")[0].innerHTML = dataObject.AverageMagnitude.toString();
		templateElement.getElementsByClassName("AverageVector")[0].innerHTML = dataObject.AverageVector.toString();
		templateElement.getElementsByClassName("AngularStandardDeviation")[0].innerHTML = dataObject.StandardAngularDeviationDegrees.toString();
		templateElement.getElementsByClassName("AverageAngular")[0].innerHTML = dataObject.MeanAngleDegrees.toString();
		templateElement.getElementsByClassName("MinAngular")[0].innerHTML = dataObject.MinAngleDegrees.toString();
		templateElement.getElementsByClassName("FirstQuartileAngular")[0].innerHTML = dataObject.FirstAngularQuartileDegrees.toString();
		templateElement.getElementsByClassName("SecondQuartileAngular")[0].innerHTML = dataObject.SecondAngularQuartileDegrees.toString();
		templateElement.getElementsByClassName("ThirdQuartileAngular")[0].innerHTML = dataObject.ThirdAngularQuartileDegrees.toString();
		templateElement.getElementsByClassName("MaxAngular")[0].innerHTML = dataObject.MaxAngleDegrees.toString();

		var whiskerPlot = templateElement.getElementsByClassName("WhiskerPlot")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegrees,
			dataObject.FirstAngularQuartileDegrees,
			dataObject.SecondAngularQuartileDegrees,
			dataObject.ThirdAngularQuartileDegrees,
			dataObject.MaxAngleDegrees,
			dataObject.MeanAngleDegrees,
			dataObject.StandardAngularDeviationDegrees
			));




		templateElement.getElementsByClassName("XYMean")[0].innerHTML = dataObject.MeanAngleDegreesXY;
		templateElement.getElementsByClassName("XYSD")[0].innerHTML = dataObject.StandardAngularDeviationDegreesXY;
		templateElement.getElementsByClassName("XYMin")[0].innerHTML = dataObject.MinAngleDegreesXY;
		templateElement.getElementsByClassName("XYQ1")[0].innerHTML = dataObject.FirstAngularDegreesXY;
		templateElement.getElementsByClassName("XYQ2")[0].innerHTML = dataObject.SecondAngularDegreesXY;
		templateElement.getElementsByClassName("XYQ3")[0].innerHTML = dataObject.ThirdAngularDegreesXY;
		templateElement.getElementsByClassName("XYMax")[0].innerHTML = dataObject.MaxAngleDegreesXY;

		whiskerPlot = templateElement.getElementsByClassName("XYWhisker")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegreesXY,
			dataObject.FirstAngularDegreesXY,
			dataObject.SecondAngularDegreesXY,
			dataObject.ThirdAngularDegreesXY,
			dataObject.MaxAngleDegreesXY,
			dataObject.MeanAngleDegreesXY,
			dataObject.StandardAngularDeviationDegreesXY
			));




		templateElement.getElementsByClassName("XZMean")[0].innerHTML = dataObject.MeanAngleDegreesXZ;
		templateElement.getElementsByClassName("XZSD")[0].innerHTML = dataObject.StandardAngularDeviationDegreesXZ;
		templateElement.getElementsByClassName("XZMin")[0].innerHTML = dataObject.MinAngleDegreesXZ;
		templateElement.getElementsByClassName("XZQ1")[0].innerHTML = dataObject.FirstAngularDegreesXZ;
		templateElement.getElementsByClassName("XZQ2")[0].innerHTML = dataObject.SecondAngularDegreesXZ;
		templateElement.getElementsByClassName("XZQ3")[0].innerHTML = dataObject.ThirdAngularDegreesXZ;
		templateElement.getElementsByClassName("XZMax")[0].innerHTML = dataObject.MaxAngleDegreesXZ;

		whiskerPlot = templateElement.getElementsByClassName("XZWhisker")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegreesXZ,
			dataObject.FirstAngularDegreesXZ,
			dataObject.SecondAngularDegreesXZ,
			dataObject.ThirdAngularDegreesXZ,
			dataObject.MaxAngleDegreesXZ,
			dataObject.MeanAngleDegreesXZ,
			dataObject.StandardAngularDeviationDegreesXZ
			));





		templateElement.getElementsByClassName("YZMean")[0].innerHTML = dataObject.MeanAngleDegreesYZ;
		templateElement.getElementsByClassName("YZSD")[0].innerHTML = dataObject.StandardAngularDeviationDegreesYZ;
		templateElement.getElementsByClassName("YZMin")[0].innerHTML = dataObject.MinAngleDegreesYZ;
		templateElement.getElementsByClassName("YZQ1")[0].innerHTML = dataObject.FirstAngularDegreesYZ;
		templateElement.getElementsByClassName("YZQ2")[0].innerHTML = dataObject.SecondAngularDegreesYZ;
		templateElement.getElementsByClassName("YZQ3")[0].innerHTML = dataObject.ThirdAngularDegreesYZ;
		templateElement.getElementsByClassName("YZMax")[0].innerHTML = dataObject.MaxAngleDegreesYZ;

		whiskerPlot = templateElement.getElementsByClassName("YZWhisker")[0];
		RenderBoxAndWhisker(whiskerPlot, 0, 0, new BoxAndWhiskerStatistics(
			dataObject.MinAngleDegreesYZ,
			dataObject.FirstAngularDegreesYZ,
			dataObject.SecondAngularDegreesYZ,
			dataObject.ThirdAngularDegreesYZ,
			dataObject.MaxAngleDegreesYZ,
			dataObject.MeanAngleDegreesYZ,
			dataObject.StandardAngularDeviationDegreesYZ
			));
	}
}