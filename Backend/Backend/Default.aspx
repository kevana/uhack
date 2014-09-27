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
    <script src="Scripts/kudos.js"></script>
</asp:Content>
