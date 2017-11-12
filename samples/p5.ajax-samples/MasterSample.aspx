<%@ Page 
    Title="" 
    Language="C#" 
    MasterPageFile="~/MasterSample.Master" 
    AutoEventWireup="true" 
    CodeBehind="WebForm1.aspx.cs" 
    Inherits="p5.samples.MasterSample" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" />

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p5:Literal 
        runat="server"
        id="foo"
        onclick="foo_onclick"
        Element="button">Click me!</p5:Literal>    
</asp:Content>