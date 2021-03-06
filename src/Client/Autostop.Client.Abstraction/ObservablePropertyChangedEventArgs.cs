﻿namespace Autostop.Client.Abstraction
{
    public class ObservablePropertyChangedEventArgs<TSender>
    {
        public ObservablePropertyChangedEventArgs(TSender sender, string propertyName)
        {
            PropertyName = propertyName;
            Sender = sender;
        }

        public string PropertyName { get; }

        public TSender Sender { get; }
    }
}