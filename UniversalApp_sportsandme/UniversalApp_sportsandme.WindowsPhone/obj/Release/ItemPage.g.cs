﻿

#pragma checksum "C:\Users\Ivan\Source\Workspaces\MobileApp_study\UniversalApp_sportsandme\UniversalApp_sportsandme\UniversalApp_sportsandme.WindowsPhone\ItemPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "86471C5604B01B329D6C1A00D918B319"
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
    partial class ItemPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 60 "..\..\ItemPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.MatchPlayer_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 61 "..\..\ItemPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AdditionButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 62 "..\..\ItemPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.IncidentsButton_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 37 "..\..\ItemPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.MainItemBack_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


