<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NextMeetingUserControl.ascx.cs" Inherits="GS.WebParts.NextMeetingUserControl, GS.WebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=76fad1f12ae5d8a7" %>

<div id="alert">
    <div class="alert_col">
        <div class="alert_col_body">
            <% if (string.IsNullOrEmpty(ErrorMessage))
               { %>

            <% if (IsNextMeeting)
               { %>
            <div class="alert_col_caption">
                <a href="<%= MeetingUrl %>" class="alert_titleLink"><%= Title %></a>
            </div>
            <div class="alert_col_text">
                Заседание №<b><%= MeetingNumber %></b><br />
                <b><%= MeetingDate.ToString("d MMMM yyyy") %></b> начало в <b><%= MeetingDate.ToString("HH:mm") %></b>
            </div>
            <div class="clear" style="height: 14px;"></div>
            <div class="alert_foot_panel">
                <div class="alert_foot">
                    <a href="<%= MeetingUrl %>" class="alert_goto"><span>Перейти<br />
                        к повестке</span></a>
                    <div class="alert_house"><%= MeetingPlace %></div>
                </div>
            </div>
            <% }
               else
               { %>
            <div class="alert_col_caption">
                <%= Title %>
            </div>
            <div class="alert_col_text alert_empty">
                <span>Заседание не назначено</span>
            </div>
            <div class="clear" style="height: 14px;"></div>
            <div class="alert_foot_panel">
                <div class="alert_foot">
                    <div class="alert_house alert_empty">
                        Место заседания<br>
                        не определено
                    </div>
                </div>
            </div>
            <% } %>


            <% }
               else
               { %>
            <div style="color: red"><%= ErrorMessage %></div>
            <% } %>
        </div>
    </div>
</div>
