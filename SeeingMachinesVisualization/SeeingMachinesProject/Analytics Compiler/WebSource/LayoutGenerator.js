
function GetTemplateCopy(templateName) {
	return document.getElementById(templateName + "Template").cloneNode(true);
}

function LayoutGenerator(tab) {

	this.targetTab = tab;

	this.stageNames = [];
	this.stageObjects = [];

	this.addProcedureDataToStage = function (stageName, procedureData) {
		var stageIndex = this.stageNames.indexOf(stageName);

		if (stageIndex == -1) {
			this.stageNames.push(stageName);
			this.stageObjects.push(GetTemplateCopy("StageLabel"));

			stageIndex = this.stageNames.length - 1;
			this.stageObjects[stageIndex].getElementsByClassName("LabelTarget")[0].innerHTML = stageName;
		}

		var stageObject = this.stageObjects[stageIndex];
		var representationObject = GetTemplateCopy(procedureData.TypeLabel);
		$(representationObject).addClass('ProcedureData');
		$(representationObject).removeAttr("id");
		ProcedureResponders[procedureData.TypeLabel](procedureData, representationObject);

		var dataContainer = stageObject.getElementsByClassName('DataContainer')[0];
		var subContainer = $("<div class='ProcedureContainer'>").get(0);
		subContainer.appendChild($("<h4 class='ProcedureLabel'>" + procedureData.Label + "</h3>").get(0));
		subContainer.appendChild(representationObject);

		dataContainer.appendChild(subContainer);
	}

	this.generateLayout = function () {
		for (var i = 0; i < this.stageNames.length; i++) {
			this.targetTab.contentElement.appendChild(this.stageObjects[i]);
		}
	}
}