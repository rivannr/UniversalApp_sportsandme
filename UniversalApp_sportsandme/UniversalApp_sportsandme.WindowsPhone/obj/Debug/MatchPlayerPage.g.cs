﻿

#pragma checksum "C:\Users\Ivan\Source\Workspaces\MobileApp_study\UniversalApp_sportsandme\UniversalApp_sportsandme\UniversalApp_sportsandme.WindowsPhone\MatchPlayerPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FEFC3923EFF02DDCD63022DDFA5DC5C9"
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
                #line 69 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.TeamComboBoxEd_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 93 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AcceptButtonEd_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 94 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.DeleteButtonEd_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 95 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BackButtonEd_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 35 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.TeamComboBox_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 44 "..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.GroupSection_ItemClick;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


