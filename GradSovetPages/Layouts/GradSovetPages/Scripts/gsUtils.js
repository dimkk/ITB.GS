var SC;
(function (SC) {
    SC.OnLoaded = function (onLoaded) {
        if (allLists) {
            onLoaded();
            return;
        }
        if (onLoaded)
            subscribers.push(onLoaded);
    };

    SC.GetList = function (name) {
        checkLoad();

        var n1 = name.toLowerCase();
        var n2 = n1 + 'list';
        var findLists = $.grep(allLists, function (e) {
            return e.name == n1 || e.name == n2;
        });

        if (findLists.length != 1)
            throw new Error('Список ' + name + ' не найден');

        return findLists[0];
    };

    SC.GetItems = function (listName, query, include) {
        var list = SC.GetList(listName);
        var items = list.getItems(query);
        if (include)
            SC.Context.load(items, include);
        else
            SC.Context.load(items);
        return items;
    };

    SC.Execute = function (success, error) {
        SC.Context.executeQueryAsync(success, error);
    };

    function checkLoad() {
        if (!allLists)
            throw new Error('Список allLists ещё не инициализирован');
    }

    var subscribers = [];
    var allLists;
    function loadLists() {
        var lists = SC.Context.get_web().get_lists();
        SC.Context.load(lists, 'Include(Id,EntityTypeName)');
        SC.Context.executeQueryAsync(function () {
            allLists = lists.get_data();
            allLists.forEach(function (e) {
                e.name = e.get_entityTypeName().toLowerCase();
            });
            if (subscribers.length > 0)
                for (var i = 0; i < subscribers.length; i++)
                    subscribers[i]();
        }, function (sender, args) {
            throw new Error('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
        });
    }

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SC.Context = SP.ClientContext.get_current();
        loadLists();
    }, 'sp.js');

})(SC || (SC = {}));

var gsUtils;

(function (gsUtils) {

    gsUtils.getSelectedWebPartId = function () {
        var e = document.getElementById("_wpSelected");
        if (!e) return null;

        var selectedId = e.getAttribute("value");
        if (!selectedId) return null;

        selectedId = selectedId.substr(12);
        e = document.getElementById(selectedId);
        if (!e) return null;

        var wpId;
        if (window._spWebPartComponents &&
            window._spWebPartComponents[selectedId]) {
            wpId = window._spWebPartComponents[selectedId].storageId;
        }
        else {
            wpId = e.getAttribute("WebPartID");
        }

        return wpId;
    };

    gsUtils.getURLParam = function (url, paramName) {
        var splitted = url.split('?');
        var params = splitted[1] ? splitted[1].split('&') : null;
        if (!params) return null;
        
        for (var i = 0; i < params.length; i++) {
            var p = params[i].split('=');
            if (p[0] !== paramName) continue;

            return p[1];
        }

        return null;
    };

    gsUtils.getIDParamFromUrl = function(url) {
        return gsUtils.getURLParam(url, "ID");
    };

})(gsUtils || (gsUtils = {}));