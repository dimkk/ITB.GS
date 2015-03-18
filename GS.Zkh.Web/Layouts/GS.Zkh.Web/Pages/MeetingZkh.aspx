<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeetingZkh.aspx.cs" Inherits="GS.Zkh.Web.MeetingZkh" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="../Scripts/typeahead.min.js"></script>
    <script type="text/javascript" src="../Scripts/knockout-2.3.0.js"></script>
    <script type="text/javascript" src="../Scripts/moment-with-langs.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment-datepicker.min.js"></script>
    <script type="text/javascript" src="../Scripts/moment-datepicker-ko.js"></script>
    <script type="text/javascript" src="/_layouts/15/SP.RequestExecutor.js"></script>

    <!-- Добавьте свои стили CSS в следующий файл -->
    <link rel="stylesheet" type="text/css" href="../Content/typeahead.js-bootstrap.css" />
    <link rel="stylesheet" type="text/css" href="../Content/moment-datepicker/datepicker.css" />
    <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

    <!-- Добавьте свой код JavaScript в следующий файл -->
    <script type="text/javascript" src="../Scripts/spin.min.js"></script>
    <script type="text/javascript" src="../Scripts/camljs.js"></script>
    <script type="text/javascript" src="../Scripts/Model/MeetingZkh.js"></script>
    <script type="text/javascript">
        var modelMetaData = {
            meeting: { listName: "MeetingZkhList", fields: [] },
            meetingAttachment: { listName: "MeetingAttachmentZkhList", fields: [] },
            agendaQuestion: { listName: "IssueZkhList", fields: [] },
        };
    </script>
    <style>
        input:disabled, select:disabled {
            color: #000;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%-- Контейнер для всех элементов формы --%>
    <div class="container topspace">
        <%-- Вкладки --%>
        <ul id="tabs" class="nav nav-tabs">
            <li class="active"><a href="#tab-common" data-toggle="tab">Общая информация</a></li>
            <li class=""><a href="#tab-additional" data-toggle="tab">Дополнительная информация</a></li>
        </ul>

        <%-- Содержимое вкладок --%>
        <div id="tabContent" class="tab-content">
            <div id="tab-common" class="tab-pane fade active in">
                <div class="form-horizontal" role="form">
                    <div class="form-group topspace">
                        <div class="col-lg-2">
                            <label style="font-weight: normal">Дата проведения:</label>
                            <label data-bind="text: meeting().MeetingDateZkh"></label>
                        </div>
                        <div class="col-lg-5">
                            <label style="font-weight: normal">Место проведения:</label>
                            <label data-bind="text: meeting().MeetingPlaceZkh"></label>
                        </div>
                    </div>
                    <div class="form-group">
                    </div>
                    <div class="form-group" data-bind="style: { display: scanAttach().FileUrl() != '' ? 'block' : 'none' }">
                        <label class="col-lg-2 control-label">Электронная версия протокола</label>
                        <div class="col-lg-10">
                            <a data-bind="attr: { href: scanAttach().FileUrl, target: '_blank' }, text: scanAttach().FileName"></a>
                            <button type="button" class="btn btn-default" data-bind="click: deleteScanAttach, enable: editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                        </div>
                    </div>
                </div>
                <%-- Таблица вопросов повестки --%>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Вопросы</h3>
                    </div>
                    <div class="panel-body" id="AgendaQuestionTableDiv">
                        <table class="table evenodd" id="AgendaQuestionTableTable">
                            <thead>
                                <tr>
                                    <% if (IsIssueEditAccessible)
                                       { %>
                                    <th></th>
                                    <% } %>
                                    <th>Номер</th>
                                    <th>Адрес</th>
                                    <th>Инвестор</th>
                                    <th>Описание вопроса</th>
                                    <th>Докладчики</th>
									<th></th>
                                    <th></th>
                                    <th></th>
                                    <% if (IsIssueEditAccessible)
                                       { %>
									<th></th>
                                    <% } %>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: agendaQuestions">
                                <tr>
                                    <% if (IsIssueEditAccessible)
                                       { %>
									<td style="white-space: nowrap">
										<button type="button" class="btn btn-default" data-bind="click: $parent.moveUpAgendaQuestion, enable: $parent.canMoveUpAgendaQuestion($data)" title="Передвинуть вверх" style="margin:0"><span class="glyphicon glyphicon-arrow-up"/></button>
										<button type="button" class="btn btn-default" data-bind="click: $parent.moveDownAgendaQuestion, enable: $parent.canMoveDownAgendaQuestion($data)" title="Передвинуть вниз" style="margin:0"><span class="glyphicon glyphicon-arrow-down"/></button>
									</td>
                                    <% } %>
                                    <td data-bind="text: IssueNumberZkh"></td>
                                    <td data-bind="text: IssueAddressZkh"></td>
                                    <td data-bind="text: IssueInvestorZkh"></td>
                                    <td data-bind="text: IssueDescriptionZkh"></td>
                                    <td data-bind="text: calcReporters"></td>
                                    <td>
                                    </td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: showAttachments" title="Посмотреть вложения вопроса" style="margin:0"><span class="glyphicon glyphicon-paperclip"></span></button>
                                    </td>
                                    <td>
										<button type="button" class="btn btn-default" data-bind="click: gotoEditQuestion" title="Посмотреть вопрос" style="margin:0"><span class="glyphicon glyphicon-edit"/></button>
                                    </td>
                                    <% if (IsIssueEditAccessible)
                                       { %>
									<td>
										<button type="button" class="btn btn-default" data-bind="click: $parent.removeAgendaQuestion" title="Удалить вопрос" style="margin:0"><span class="glyphicon glyphicon-remove"/></button>
									</td>
                                    <% } %>
                                </tr>
                            </tbody>
                        </table>
                        <% if (IsIssueEditAccessible)
                           { %>
						<button type="button" class="btn btn-default" data-bind="click: createIssueRg">Создать</button>
                        <% } %>
                    </div>
                </div>
                <%-- Таблица вложений заседания --%>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Вложения</h3>
                    </div>
                    <div class="panel-body">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th class="col-md-3">Файл</th>
                                    <th class="col-md-2">Тип документа</th>
                                    <th class="col-md-6">Описание</th>
                                    <th class="col-md-1"></th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: attachments, afterRender: $root.initFileReader()">
                                <tr>
                                    <td>
                                        <a data-bind="attr: { href: FileUrl, target: '_blank' }, text: FileName, visible: FileUrl() != ''"></a>
                                        <input class="form-control" data-bind="text: FilePath, visible: FileUrl() == '', event: { change: function (data, event) { selectedFile($element, data, event) } }, enable: $parent.editEnabled" type='file' />
                                    </td>
                                    <td>
                                        <select class="form-control" placeholder="Выберите значение" data-bind="
    options: $root.availableAttachDocTypes,
    optionsText: 'name',
    optionsValue: 'id',
    value: DType,
    enable: $parent.editEnabled">
                                        </select>
                                    </td>
                                    <td>
                                        <textarea class="form-control" data-bind="value: Descr, enable: $parent.editEnabled"></textarea>
                                    </td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: $parent.removeAttach, enable: $parent.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <%--<button class="btn btn-primary" data-bind="click: addAttach, enable: editEnabled">Добавить</button>--%>
                    </div>
                </div>
            </div>
            <div id="tab-additional" class="tab-pane fade">
                <div class="form-horizontal" role="form">
                    <div class="form-group topspace">
                        <label for="inputNumber" class="col-lg-2 control-label">Номер</label>
                        <div class="col-lg-3">
                            <input data-bind="value: meeting().MeetingNumberZkh, enable: editEnabled" type="text" class="form-control" id="inputNumber" placeholder="Номер заседания">
                        </div>
                        <label for="inputStatus" class="col-lg-2 control-label">Статус</label>
                        <div class="col-lg-3">
                            <select id="inputStatus" class="form-control" placeholder="Выберите значение" data-bind="
    options: $root.availableMeetingStatuses,
    optionsText: 'name',
    optionsValue: 'name',
    value: meeting().MeetingStatusZkh,
    enable: editEnabled">
                            </select>
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Список членов комиссии</h3>
                    </div>
                    <div class="panel-body">
                        <table id="other-participant-table" class="table">
                            <thead>
                                <tr>
                                    <th>Роль</th>
                                    <th>ФИО</th>
                                    <th>Должность</th>
                                    <th>Организация</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody data-bind="foreach: meeting().additionalParticipants">
                                <tr>
                                    <td data-bind="text: ParticipantRole"></td>
                                    <td data-bind="text: ParticipantFullName"></td>
                                    <td data-bind="text: ParticipantPosition"></td>
                                    <td data-bind="text: ParticipantOrg"></td>
                                    <td>
                                        <button type="button" class="btn btn-default" data-bind="click: $root.meeting().removeAdditionalPartcipant, enable: $root.editEnabled"><span class="glyphicon glyphicon-trash"></span></button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div>
                            <button type="button" class="btn btn-primary" data-bind="click: meeting().selectAdditionalParticipants, enable: $root.editEnabled" id="addAdditionalParticipants" name="addAdditionalParticipants">Добавить</button>
                            <input id="hidden-additioanl-participants" style="display: none;" data-bind="value: meeting().AdditionalParticipantsInput" type="text" />
                        </div>
                    </div>
                </div>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Список остальных участников</h3>
                    </div>
                    <div class="panel-body">
                        <textarea data-bind="value: meeting().OtherParticipantsList, enable: $root.editEnabled" rows="4" class="form-control"></textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="container">
            <button class="btn btn-default" data-bind="click: closeForm" id="oCancelButton" name="oCancelButton" style="float:right">Закрыть</button>
            <button class="btn btn-default" data-bind="click: exportToWord" name="bExportToWord" style="float:right">Экспорт в Word</button>
        </div>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Заседание МВК.Земля
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Заседание МВК.Земля
</asp:Content>
