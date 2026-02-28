namespace Fitliyo.Permissions;

public static class FitliyoPermissions
{
    public const string GroupName = "Fitliyo";

    public static class Trainers
    {
        public const string Default = GroupName + ".Trainers";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Verify = Default + ".Verify";
    }

    public static class Packages
    {
        public const string Default = GroupName + ".Packages";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Categories
    {
        public const string Default = GroupName + ".Categories";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Orders
    {
        public const string Default = GroupName + ".Orders";
        public const string Cancel = Default + ".Cancel";
    }

    public static class Reviews
    {
        public const string Default = GroupName + ".Reviews";
        public const string Delete = Default + ".Delete";
    }

    public static class Messaging
    {
        public const string Default = GroupName + ".Messaging";
    }

    public static class Subscriptions
    {
        public const string Default = GroupName + ".Subscriptions";
        public const string ManagePlans = Default + ".ManagePlans";
    }

    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
    }

    public static class Admin
    {
        public const string Dashboard = GroupName + ".Admin.Dashboard";
        public const string UserManagement = GroupName + ".Admin.UserManagement";
    }
}
