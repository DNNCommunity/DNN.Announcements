using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;

namespace DotNetNuke.Modules.Announcements.Components.Business
{
    public class SubscriberController
    {

        public static void AddSubscriber(AnnouncementsSubscriberInfo subscriber)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementsSubscriberInfo>();

                repository.Insert(subscriber);
            }
        }

        public static void UpdateSubscriber(AnnouncementsSubscriberInfo subscriber)
        {

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementsSubscriberInfo>();
                repository.Update(subscriber);

            }

        }

        public static void DeleteSubscriber(int userId, int moduleId)
        {

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementsSubscriberInfo>();
                repository.Delete("WHERE UserID = @0 AND ModuleID = @1", userId, moduleId);
            }
        }

        public static AnnouncementsSubscriberInfo GetSubscriber(int userId, int moduleId)
        {
            AnnouncementsSubscriberInfo subscriber;
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementsSubscriberInfo>();
                subscriber = repository.Find("WHERE ModuleID = @0 AND UserID=@1", moduleId, userId).FirstOrDefault();
            }
            return subscriber;
        }

        public static IEnumerable<AnnouncementsSubscriberInfo> GetSubscribers(int moduleId)
        {

            IEnumerable<AnnouncementsSubscriberInfo> subscribers;

            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<AnnouncementsSubscriberInfo>();
                subscribers =
                    repository.Find("WHERE ModuleID = @0",moduleId);
            }
            return subscribers;
        }
    }
}