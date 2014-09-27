<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Backend._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">

</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    <asp:Button ID="myButton" Text="Nomination" OnClientClick="return createNomination()" runat="server" />
    <br />
    <asp:Button ID="Button1" Text="Rating" OnClientClick="return createRating()" runat="server" />
    <br />
    <asp:Button ID="Button2" Text="Get Feed" OnClientClick="return getFeed()" runat="server" />
    <br />
    <asp:Button ID="Button3" Text="Get 'All' Feed" OnClientClick="return getFeedPast()" runat="server" />
    <br />
    <asp:Button ID="Button4" Text="Get Issued" OnClientClick="return getBadges()" runat="server" />
    <br />
    <div id="output">
    </div>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
    <script src="Scripts/moment.js"></script>
    <script>
        var previousTime = new Date();
        $(document).ready(function () {
            previousTime = new Date();
        });
        function createNomination() {

            var dataVal = JSON.stringify({
                "user_first_name": "Bryan",
                "user_last_name": "Stadick",
                "badge_id": "abcd123456789",
                "badge_name": "Debugging Badge",
                "badge_description": "Awarded for debugging your code",
                "badge_url": "http://waynesword.palomar.edu/images/rainbe1b.jpg",
                "nomination_description_short": "Bryan came, saw, and debugged.",
                "nomination_description_long": "This description is slightly longer than the previous description.",
                "req_vote_count": 50,
                "organization_id": "123456789abcd"
            });

            $.ajax({
                type: "POST",
                url: "Default.aspx/CreateNomination",
                data: dataVal,
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    returnValue = false;
                },
                success: function (data) {
                    $("#output").append("<p>" + data.d + "</p>");
                }
            });

            return false;
        }
        function createRating() {

            var dataVal = JSON.stringify({
                "nomination_id": 0,
                "user_id": 0,
                "review": "Bryan is the best!"
            });

            $.ajax({
                type: "POST",
                url: "Default.aspx/CreateRating",
                data: dataVal,
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    returnValue = false;
                },
                success: function (data) {
                    $("#output").append("<p>" + data.d + "</p>");
                }
            });

            return false;
        }
        function getFeed() {

            var tr = new Date();
            time = previousTime.toISOString();
            time = time.replace("T", " ");
            time = time.split(".", 1);

            previousTime = tr;

            var dataVal = JSON.stringify({
                "datetime": "'" + time[0] + "'"
            });

            $.ajax({
                type: "POST",
                url: "Default.aspx/GetFeed",
                data: dataVal,
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    returnValue = false;
                },
                success: function (data) {
                    $("#output").append("<p>" + data.d + "</p>");
                }
            });

            return false;
        }
        function getFeedPast() {

            var time = new Date();
            var m = moment(time);
            m.add('days', -10);
            time = m.format();
            time = time.replace("T", " ");
            var ind = time.lastIndexOf("-");
            time = time.substr(0, ind);

            alert(time);


            var dataVal = JSON.stringify({
                "datetime": "'" + time + "'"
            });

            $.ajax({
                type: "POST",
                url: "Default.aspx/GetFeed",
                data: dataVal,
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    returnValue = false;
                },
                success: function (data) {
                    $("#output").append("<p>" + data.d + "</p>");
                }
            });

            return false;
        }
        function getBadges() {

            var dataVal = JSON.stringify({
                "organization_id": ""
            });

            $.ajax({
                type: "POST",
                url: "Default.aspx/GetBadges",
                data: dataVal,
                contentType: 'application/json; charset=utf-8',
                dataType: "json",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
                    returnValue = false;
                },
                success: function (data) {
                    $("#output").append("<p>" + data.d + "</p>");
                }
            });

            return false;
        }
    </script>
</asp:Content>
