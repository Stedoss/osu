// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.IO;
using osu.Framework.Platform;
using osu.Game.Overlays;
using osu.Game.Overlays.Notifications;

namespace osu.Game.Online.Chat
{
    public class ChannelContentExporter
    {
        private readonly Storage chatStorage;

        private readonly NotificationOverlay notifications;

        public ChannelContentExporter(Storage storage, NotificationOverlay notifications)
        {
            chatStorage = storage.GetStorageForDirectory(@"chat");
            this.notifications = notifications;
        }

        public void ExportChannel(Channel channel)
        {
            var dt = DateTime.Now;

            string filename = $"{channel.Name}-{dt:yyyyMMdd-HHmmss}.txt";

            using (var stream = chatStorage.GetStream(filename, FileAccess.Write, FileMode.Create))
            using (var sw = new StreamWriter(stream))
            {
                foreach (var message in channel.Messages)
                {
                    string formattedMessage = $"{message.Timestamp.LocalDateTime:HH:mm:ss} {message.Sender}: {message.Content}";
                    sw.WriteLine(formattedMessage);
                }
            }

            notifications.Post(new SimpleNotification
            {
                Text = $"Exported chat log to chat/{filename}.",
                Activated = () =>
                {
                    chatStorage.PresentFileExternally(filename);
                    return true;
                }
            });
        }
    }
}
