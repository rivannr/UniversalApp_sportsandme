﻿

#pragma checksum "D:\111\UniversalApp_sportsandme\UniversalApp_sportsandme\UniversalApp_sportsandme.Windows\RedCardPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "98696B3907487924B0803DB33F19880D"
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
    partial class RedCardPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 91 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.TeamComboBoxEd_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 107 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.MinuteTextBox_TextChanged;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 109 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.AdditionMinuteTextBox_TextChanged;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 113 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AcceptButtonEd_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 114 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.DeleteButtonEd_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 115 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BackButtonEd_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 40 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Selector)(target)).SelectionChanged += this.TeamComboBox_SelectionChanged;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 44 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AddButton_Click;
                 #line default
                 #line hidden
                break;
            case 9:
                #line 50 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.GroupSection_ItemClick;
                 #line default
                 #line hidden
                break;
            case 10:
                #line 31 "..\..\..\RedCardPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.CancelButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


