﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Phone.Controls;

namespace MyFriendsAround.WP7.Views
{
    public class ExceptionPrompt: PopUp<Exception, PopUpResult>
    {

        private Button okButton;
        private CheckBox submitCheckBox;
        private Exception exception;

        public ExceptionPrompt()
        {
            DefaultStyleKey = typeof(ExceptionPrompt);
            DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (okButton != null)
                okButton.Click -= okButton_Click;

            okButton = GetTemplateChild("okButton") as Button;
            submitCheckBox = GetTemplateChild("canSubmitCheckBox") as CheckBox;

            if (okButton != null)
                okButton.Click += okButton_Click;
        }

        public string To { get; set; }

        void okButton_Click(object sender, RoutedEventArgs e)
        {
            var message = new StringBuilder();
            message.Append("Exception type: ");
            message.Append(exception.GetType());
            message.Append(Environment.NewLine);
            message.Append("Message: ");
            message.Append(exception.Message);
            message.Append(Environment.NewLine);
            message.Append("Stack trace: ");
            message.Append(exception.StackTrace);
            message.ToString();

            var task = new Microsoft.Phone.Tasks.EmailComposeTask { Body = message.ToString(), Subject = "Error Report", To = To };

            if (submitCheckBox.IsChecked == true)
            {
                task.Show();
            }

            OnCompleted(new PopUpEventArgs<Exception, PopUpResult> { PopUpResult = PopUpResult.Ok });
        }

        public void Show(Exception exception)
        {
            this.exception = exception;
            base.Show();
        }

    }
}
