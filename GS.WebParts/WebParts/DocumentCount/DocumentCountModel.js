'use strict';
var DocumentCount;
(function (DocumentCount) {
    var spinOptions = {
        lines: 13,
        length: 20,
        width: 10,
        radius: 30,
        corners: 1,
        rotate: 0,
        direction: 1,
        color: '#000',
        speed: 1.7,
        trail: 60,
        shadow: false,
        hwaccel: false,
        className: 'spinner',
        zIndex: 2e9,
        top: 'auto',
        left: 'auto'
    };

    var Document = (function () {
        function Document(data) {
            this.Name = data.Name;
			this.IsSubElement = data.IsSubElement;
			this.Filter = data.Filter;
            this.AllCount = data.AllCount;
            this.WorkCount = data.WorkCount;
            this.IssuedCount = data.IssuedCount;
            this.ExpiredCount = data.ExpiredCount;
            this.InhabitedCount = data.InhabitedCount;
            this.UnInhabitedCount = data.UnInhabitedCount;
			this.AllLink = data.AllLink;
			this.WorkLink = data.WorkLink;
			this.IssuedLink = data.IssuedLink;
			this.ExpiredLink = data.ExpiredLink;
			this.InhabitedLink = data.InhabitedLink;
			this.UninhabitedLink = data.UninhabitedLink;
        }
        return Document;
    })();
    DocumentCount.Document = Document;

    var Model = (function () {
        function Model() {
            this.Documents = ko.observableArray([]);
            this.spinner = new Spinner(spinOptions);
        }

        Model.prototype.spin = function (show) {
            var target = document.getElementById("PanelContent");
            if (!target)
                return;

            if (show) {
                $(target.children).each(function (index, elem) {
                    $(elem).hide();
                });
                $(target).css("min-height", "150px");
                this.spinner.spin(target);
            } else {
                this.spinner.stop();
                $(target.children).each(function (index, elem) {
                    $(elem).show();
                });
                $(target).css("min-height", "0px");
            }
        };

		Model.prototype.GetByType = function (docs, typeId) {
			var item;
			return $.grep(docs, function (e) {
				item = e.get_item('DtpNameOfQuestion');
				return item && $.inArray(item.get_lookupId(), typeId) >= 0;
			});
		}
		
		Model.prototype.GetGpzu = function (allDocs) {
			var name = 'Градостроительный план земельного участка';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [1]);
			return new Document({
				Name: name,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
				});
		}

		Model.prototype.GetPpt = function (allDocs) {
			var name = 'Подготовка, согласование, утверждение и выдача документации по планировке территорий';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [2,3]);
			return new Document({
				Name: name,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}

		Model.prototype.GetPptDevelop = function (allDocs) {
			var name = 'О подготовке документации по планировке территории';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [2]);
			return new Document({
				Name: name,
				IsSubElement: true,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}

		Model.prototype.GetPptApprove = function (allDocs) {
			var name = 'Об утверждении проекта планировки территории';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [3]);
			return new Document({
				Name: name,
				IsSubElement: true,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}
		
		Model.prototype.GetLicense = function (allDocs) {
			var name = 'Разрешение на строительство';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [4,5,6]);
			return new Document({
				Name: name,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}

		Model.prototype.GetLicenseBuild = function (allDocs) {
			var name = 'Выдача разрешения на строительство (реконструкцию)';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [4]);
			return new Document({
				Name: name,
				IsSubElement: true,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}
		
		Model.prototype.GetLicenseStart = function (allDocs) {
			var name = 'Выдача разрешения на ввод объекта в эксплуатацию';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [6]);
			return new Document({
				Name: name,
				IsSubElement: true,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}

		Model.prototype.GetLicenseRenewal = function (allDocs) {
			var name = 'Продление срока действия разрешения на строительство';
			var baseUrl = String.format('/SitePages/DTP/{0}.aspx', name);
			var docs = this.GetByType(allDocs, [5]);
			return new Document({
				Name: name,
				IsSubElement: true,
				AllCount: docs.length,
				WorkCount: this.GetWorkCount(docs),
				IssuedCount: this.GetIssuedCount(docs),
				ExpiredCount: this.GetExpiredCount(docs),
				InhabitedCount: this.GetInhabitedCount(docs),
				UnInhabitedCount: this.GetUnInhabitedCount(docs),
				AllLink: this.GetLink(baseUrl),
				WorkLink: this.GetWorkLink(baseUrl),
				IssuedLink: this.GetIssuedLink(baseUrl),
				ExpiredLink: this.GetExpiredLink(baseUrl),
				InhabitedLink: this.GetInhabitedLink(baseUrl),
				UninhabitedLink: this.GetUnInhabitedLink(baseUrl)
			});
		}

		Model.prototype.GetFilterPart = function (index, key, value, operation) {
			var op = operation ? String.format('FilterOp{0}={1}&', index, operation) : '';
			return String.format('FilterField{0}={1}&FilterValue{0}={2}&{3}', index, key, value, op);
		}
		
		Model.prototype.GetLink = function (baseUrl, filter) {
			var filters = '';
			var index = 1;
			if (filter) {
				index += filter.length;
				for (var i = 0; i < filter.length; i++) {
					filters += this.GetFilterPart(i + 1, filter[i].Key, filter[i].Value, filter[i].Op);
				}
				//var view = $.grep(this.Views, function (e) {
				//	return e.get_serverRelativeUrl() == baseUrl;
				//});
				//if (view.length == 1) {
					//var viewId = view[0].get_id();
					//var filters = '?FilterField1=DtpDateOfRegistration&FilterValue1=2015-03-23&FilterOp1=Gt';
					//var values = String.format('FilterField{2}1={0}-FilterValue{2}1={1}', filter[0].Key, escapeProperly(escapeProperly(filter[0].Value)), filter[0].Value.indexOf(';') >= 0 ? 's' : '');
					//values = values + '-FilterField2=DtpNumber-FilterValue2=706-FilterOp2=Gt'; //+ escapeProperly(escapeProperly('2015-03-23')) + '-FilterOp2=Gt';
					//var values = 'FilterField1=DtpNumber-FilterValue1=706-FilterOp1=Gt';
					//filters = String.format(
					//	'#InplviewHash{0}={1}',
					//	viewId,
					//	values
					//);
				//}
				//else
				//	console.log('Не найдено представление ' + baseUrl);
			}
			if (this.Filters) {
				if (this.Filters.DateFrom)
					filters += this.GetFilterPart(index++, 'DtpDateOfRegistration', this.Filters.DateFrom, 'Geq');
				if (this.Filters.DateTo)
					filters += this.GetFilterPart(index++, 'DtpDateOfRegistration2', this.Filters.DateTo, 'Leq');
			}
			if (filters != '')
				filters = '?' + filters.substr(0, filters.length - 1);
			return String.format('{0}{1}', baseUrl, filters);
		}
		
		Model.prototype.GetWorkCount = function (docs) {
			return $.grep(docs, function (e) {
				return e.get_item('DtpOmsuResult') != 'Выдано';
			}).length;
		}
		
		Model.prototype.GetWorkLink = function (baseUrl) {
			//return this.GetLink(baseUrl, [{ Key: 'DtpOmsuResult', Value: 'На рассмотрении;#Просрочено' }]);
			return this.GetLink(baseUrl, [{ Key: 'DtpOmsuResult', Op: 'Neq', Value: 'Выдано' }]);
		}

		Model.prototype.GetIssuedCount = function (docs) {
			return $.grep(docs, function (e) {
				return e.get_item('DtpOmsuResult') == 'Выдано';
			}).length;
		}
		
		Model.prototype.GetIssuedLink = function (baseUrl) {
			return this.GetLink(baseUrl, [{ Key: 'DtpOmsuResult', Value: 'Выдано'}]);
		}

		Model.prototype.GetExpiredCount = function (docs) {
			return $.grep(docs, function (e) {
				return e.get_item('DtpOmsuResult') == 'Просрочено';
			}).length;
		}
		
		Model.prototype.GetExpiredLink = function (baseUrl) {
			return this.GetLink(baseUrl, [{ Key: 'DtpOmsuResult', Value: 'Просрочено'}]);
		}

		Model.prototype.GetInhabitedCount = function (docs) {
			return $.grep(docs, function (e) {
				return e.get_item('DtpObjectType') == 'Жилой';
			}).length;
		}

		Model.prototype.GetInhabitedLink = function (baseUrl) {
			return this.GetLink(baseUrl, [{ Key: 'DtpObjectType', Value: 'Жилой' }]);
		}
		
		Model.prototype.GetUnInhabitedCount = function (docs) {
			return $.grep(docs, function (e) {
				return e.get_item('DtpObjectType') == 'Нежилой';
			}).length;
		}
		
		Model.prototype.GetUnInhabitedLink = function (baseUrl) {
			return this.GetLink(baseUrl, [{ Key: 'DtpObjectType', Value: 'Нежилой' }]);
		}
		
        Model.prototype.loadData = function (filters) {
			SP.UI.Status.removeAllStatus(true);
			this.Filters = filters;
            this.Documents.removeAll();
			var self = this;
            this.spin(true);
            try  {
				SC.OnLoaded(function() {
					var list = SC.GetList('ReestrDTP');
					var views = list.get_views();
					var query = new SP.CamlQuery();
					var q, q1, q2;
					if (self.Filters) {
						if (self.Filters.DateFrom)
							q1 = String.format("<Geq><FieldRef Name='DtpDateOfRegistration'/><Value Type='DateTime'>{0}</Value></Geq>", self.Filters.DateFrom);
						if (self.Filters.DateTo)
							q2 = String.format("<Leq><FieldRef Name='DtpDateOfRegistration'/><Value Type='DateTime'>{0}</Value></Leq>", self.Filters.DateTo);
						var q;
						if (q1 && q2)
							q = String.format("<And>{0}{1}</And>", q1, q2);
						else if (q1 && !q2)
							q = q1;
						else if (!q1 && q2)
							q = q2;
						if (q)
							q = String.format("<Where>{0}</Where>", q);
					}
					query.set_viewXml(String.format('<View Scope="Recursive"><Query>{0}</Query></View>', q));
					var items = list.getItems(query);
					SC.Context.load(items, 'Include(DtpNameOfQuestion,DtpObjectType,DtpOmsuResult,DtpGsuExpired)');
					SC.Context.load(views);
					SC.Execute(function() {
						self.Views = views.get_data();
						var docs = items.get_data();
						self.Documents.push(self.GetGpzu(docs));
						self.Documents.push(self.GetPpt(docs));
						self.Documents.push(self.GetPptDevelop(docs));
						self.Documents.push(self.GetPptApprove(docs));
						self.Documents.push(self.GetLicense(docs));
						self.Documents.push(self.GetLicenseBuild(docs));
						self.Documents.push(self.GetLicenseStart(docs));
						self.Documents.push(self.GetLicenseRenewal(docs));
						self.spin(false);
					},
					function (sender, args) {
						SP.UI.Status.setStatusPriColor(SP.UI.Status.addStatus("Ошибка выполнения запроса:", String.format('<br/>{0}<br/>{1}', args.get_message(), args.get_stackTrace())), 'red');
						self.spin(false);
					});
				});
            } catch (e) {
                var err = e;
                this.ErrorMsg = ko.observable(err.message);
                this.spin(false);
            }

            return this;
        };
        return Model;
    })();
    DocumentCount.Model = Model;
})(DocumentCount || (DocumentCount = {}));
