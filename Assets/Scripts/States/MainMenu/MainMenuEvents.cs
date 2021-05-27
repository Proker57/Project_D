namespace BOYAREngine.MainMenu
{
    public static class MainMenuEvents
    {
        public delegate void CloseTabsEvent();
        public static CloseTabsEvent CloseTabs;

        public delegate void CloseTabsUpgradeEvent();
        public static CloseTabsUpgradeEvent CloseTabsUpgrade;

        public delegate void UpgradeUpdateUiEvent();
        public static UpgradeUpdateUiEvent UpgradeUpdateUi;
    }
}

