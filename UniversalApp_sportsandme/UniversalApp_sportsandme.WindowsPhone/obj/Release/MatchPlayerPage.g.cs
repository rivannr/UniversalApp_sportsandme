﻿

#pragma checksum "C:\Users\Ivan\Source\Workspaces\MobileApp_study\UniversalApp_sportsandme\UniversalApp_sportsandme\UniversalApp_sportsandme.WindowsPhone\MatchPlayerPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CE8CA87C1CA19AEDCD8907E9C8D50BAA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UniversalApp_sportsandme
{
    partial class MatchPlayerPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 34 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.TeamComboBox_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 43 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.GroupSection_ItemClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

