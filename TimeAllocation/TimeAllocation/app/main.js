var app = angular.module("myApp", []);

app.controller("myCtrl", function ($scope, $http, $sce) {


    $http.get("data/resources.ashx").then(function (response) {
        $scope.resources = response.data;
    });

    $scope.renderHTML = function (htmlCode) {
        return $sce.trustAsHtml(htmlCode);
    };

    $scope.getAllocation = function () {
        $http.get("data/allocation.ashx?rn=" + $scope.selectedDeveloper.Resource_Name).then(function (response) {

            //add columns to table
            $scope.columns = [{ "name": "PTR", "value": "PTR" },
                { "name": "Client", "value": "Client" },
                { "name": "Priority", "value": "Priority" },
                { "name": "Due Date", "value": "Dev_Due_Date" },
                { "name": "Estimate", "value": "Dev_Estimate" },
                { "name": "Completed", "value": "Hrs_Completed" },
                { "name": "Remaining", "value": "Hrs_Remaining" },
                { "name": "Status", "value": "Dev_Status" }];

            //Build the week ending column list out 6 weeks
            $http.get("data/weekending.ashx").then(function (response) {                
                for (var x in response.data) {
                    $scope.columns.push({ "name": response.data[x].Week_Ending_Text, "value": "C" + response.data[x].Week_Ending_Text.replace("/", "_").replace("/", "_"), "format": "hoursInput" });
                }
            });
                       

            $scope.allocations = response.data;
        });

    }

    $scope.saveAllocation = function () {
        $http.put("data/updateallocation.ashx", $scope.allocations).then(function (response) {
        });
    }

});