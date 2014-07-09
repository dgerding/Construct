
function BoxAndWhiskerStatistics(
	minimum,
	firstQuartile,
	secondQuartile,
	thirdQuartile,
	maximum,
	mean,
	standardDeviation
	) {

	this.minimum = minimum;
	this.firstQuartile = firstQuartile;
	this.secondQuartile = secondQuartile;
	this.thirdQuartile = thirdQuartile;
	this.maximum = maximum;

	//	Not really box-and-whisker data, but whatever
	this.mean = mean;
	this.standardDeviation = standardDeviation;
}

function RenderBoxAndWhisker(canvasTarget, x, y, statistics) {
	var maxWidth = canvasTarget.width;
	var height = 100;

	var ctx = canvasTarget.getContext('2d');
	ctx.strokeStyle = "1px solid #000";
	
	var scale;
	if (statistics.maximum == statistics.minimum)
		scale = 1;
	else
		scale = maxWidth / (statistics.maximum - statistics.minimum);

	x -= statistics.minimum * scale;

	ctx.strokeRect(x + statistics.firstQuartile * scale, y, (statistics.thirdQuartile - statistics.firstQuartile) * scale, height);
	
	ctx.beginPath();
	ctx.moveTo(x + statistics.secondQuartile * scale, y);
	ctx.lineTo(x + statistics.secondQuartile * scale, y + height);
	ctx.stroke();

	ctx.beginPath();
	ctx.moveTo(x + statistics.minimum * scale, y);
	ctx.lineTo(x + statistics.minimum * scale, y + height);
	ctx.stroke();

	ctx.beginPath();
	ctx.moveTo(x + statistics.maximum * scale, y);
	ctx.lineTo(x + statistics.maximum * scale, y + height);
	ctx.stroke();

	ctx.beginPath();
	ctx.moveTo(x + statistics.minimum * scale, y + height / 2);
	ctx.lineTo(x + statistics.firstQuartile * scale, y + height / 2);
	ctx.stroke();

	ctx.beginPath();
	ctx.moveTo(x + statistics.maximum * scale, y + height / 2);
	ctx.lineTo(x + statistics.thirdQuartile * scale, y + height / 2);
	ctx.stroke();

	var sdHeight = 10;
	ctx.strokeStyle = "#444";
	ctx.strokeRect(x + statistics.mean * scale - statistics.standardDeviation * scale / 2, y + 150 - sdHeight / 2, statistics.standardDeviation * scale, sdHeight);

	ctx.beginPath();
	ctx.fillStyle = "#F00";
	ctx.arc(x + statistics.mean * scale, y + 150, 1, 0, 2*Math.PI);
	ctx.fill();
}