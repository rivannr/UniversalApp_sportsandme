﻿

#pragma checksum "D:\111\UniversalApp_sportsandme\UniversalApp_sportsandme\UniversalApp_sportsandme.Windows\MatchPlayerPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7E452BB051BFF62A649BF45184829CAD"
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
                #line 64 "..\..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Unchecked += this.IsCaptainRB_Unchecked;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 34 "..\..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.CancelButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 38 "..\..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.TeamComboBox_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 42 "..\..\..\MatchPlayerPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.SaveButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


