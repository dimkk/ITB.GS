var SearchQuestionPage = (function ($) {
    var root = {};

    root.init = function () {

        function Question(data) {
            return {
                Id: ko.observable(data.get_item("ID")),
                IssueNumber: ko.observable(data.get_item("IssueNumber")),
                IssueTheme: ko.observable(data.get_item("IssueTheme"))
            };
        };

        var SearchQuestionModel = function () {
            var self = this;
            self.searchResults = ko.observableArray([]);
            self.searchTheme = ko.observable("");

            self.search = function () {
                var appWebContext = SP.ClientContext.get_current();
                var issueList = appWebContext.get_web().get_lists().getByTitle("Вопросы повестки заседания");
                var query = new SP.CamlQuery();
                query.set_viewXml("<View><Query><Where><Contains><FieldRef Name='IssueTheme'/><Value Type='Note'>" + self.searchTheme() + "</Value></Contains></Where></Query></View>");
                var aqInstance = issueList.getItems(query);
                appWebContext.load(aqInstance);
                appWebContext.executeQueryAsync(function () {
                    var result = [];
                    var enumerator = aqInstance.getEnumerator();
                    while (enumerator.moveNext()) {
                        result.push(new Question(enumerator.get_current()));
                    }
                    self.searchResults(result);
                });
            };

            self.CommitPopUp = function () {
                window.frameElement.commitPopup(ko.toJSON(this));
                return false;
            };
            
            self.ClosePopUp = function () {
                window.frameElement.cancelPopUp();
                return false;
            };
        };
        
        ko.applyBindings(new SearchQuestionModel());
    };
    return root;
})(jQuery);
