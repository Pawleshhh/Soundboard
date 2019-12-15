using ManiacSoundboard.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ManiacSoundboard
{
    /// <summary>
    /// Interaction logic for KeyBindingUserControl.xaml
    /// </summary>
    public partial class KeyBindingUserControl : UserControl
    {
        public KeyBindingUserControl()
        {
            InitializeComponent();
            this.GotFocus += KeyBindingUserControl_GotFocus;
        }

        #region IsSystemKeysEnabled

        public static readonly DependencyProperty IsModifiersEnabledProperty =
            DependencyProperty.Register("IsModifiersEnabled", typeof(bool), typeof(KeyBindingUserControl), new PropertyMetadata(true));

        public bool IsModifiersEnabled
        {
            get => (bool)GetValue(IsModifiersEnabledProperty);
            set => SetValue(IsModifiersEnabledProperty, value);
        }

        #endregion

        #region BoundKeysText

        public static readonly DependencyProperty BoundKeysTextProperty =
            DependencyProperty.Register("BoundKeysText", typeof(string), typeof(KeyBindingUserControl), new PropertyMetadata("None"));

        public string BoundKeysText
        {
            get => (string)GetValue(BoundKeysTextProperty);
            set => SetValue(BoundKeysTextProperty, value);
        }

        #endregion

        #region SystemKeys

        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(KeyBindingUserControl),
                new PropertyMetadata(OnModifiersChanged));

        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        private static void OnModifiersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyBindingUserControl = d as KeyBindingUserControl;
            if (keyBindingUserControl == null) return;

            keyBindingUserControl.BoundKeysText = FormsWpfKeysHelper.DescribeWpfBinding(keyBindingUserControl.TriggerKey, (ModifierKeys)e.NewValue);
        }

        #endregion

        #region BoundKey

        public static readonly DependencyProperty TriggerKeyProperty =
            DependencyProperty.Register("TriggerKey", typeof(Key), typeof(KeyBindingUserControl),
                new PropertyMetadata(OnTriggerKeyChanged));

        public Key TriggerKey
        {
            get { return (Key)GetValue(TriggerKeyProperty); }
            set { SetValue(TriggerKeyProperty, value); }
        }

        private static void OnTriggerKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyBindingUserControl = d as KeyBindingUserControl;
            if (keyBindingUserControl == null) return;

            keyBindingUserControl.BoundKeysText = FormsWpfKeysHelper.DescribeWpfBinding((Key)e.NewValue, keyBindingUserControl.Modifiers);
        }

        #endregion

        #region IsSetting

        private static readonly DependencyPropertyKey IsSettingKeysProperty =
            DependencyProperty.RegisterReadOnly("IsSettingKeys", typeof(bool), typeof(KeyBindingUserControl), null);

        public static readonly DependencyProperty IsSettingProperty = IsSettingKeysProperty.DependencyProperty;

        public bool IsSettingKeys
        {
            get => (bool)GetValue(IsSettingProperty);
            protected set => SetValue(IsSettingKeysProperty, value);
        }

        #endregion

        #region KeyCombination

        public static readonly DependencyProperty KeyCombinationProperty =
            DependencyProperty.Register("KeyCombination", typeof(KeyCombination), typeof(KeyBindingUserControl),
                new PropertyMetadata(OnKeyCombinationChanged));

        public KeyCombination KeyCombination
        {
            get => (KeyCombination)GetValue(KeyCombinationProperty);
            set => SetValue(KeyCombinationProperty, value);
        }

        private static void OnKeyCombinationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var keyBindingUserControl = d as KeyBindingUserControl;
            if (keyBindingUserControl == null) return;

            KeyCombination combination = (KeyCombination)e.NewValue;
            
            if(combination == null)
            {
                keyBindingUserControl.TriggerKey = FormsWpfKeysHelper.WpfKeyFromFormsKey(System.Windows.Forms.Keys.None);
                keyBindingUserControl.Modifiers = FormsWpfKeysHelper.WpfModifiersFromFormsModifiers(System.Windows.Forms.Keys.None);
                return;
            }

            keyBindingUserControl.TriggerKey = FormsWpfKeysHelper.WpfKeyFromFormsKey(combination.TriggerKey);
            keyBindingUserControl.Modifiers = FormsWpfKeysHelper.WpfModifiersFromFormsModifiers(CombinationHelper.GetModifiers(combination));
        }

        #endregion

        #region Option keys

        public static readonly DependencyProperty NoneKeyProperty =
            DependencyProperty.Register("NoneKey", typeof(Key), typeof(KeyBindingUserControl), new PropertyMetadata(Key.Back));

        public Key NoneKey
        {
            get { return (Key)GetValue(NoneKeyProperty); }
            set { SetValue(NoneKeyProperty, value); }
        }

        public static readonly DependencyProperty StopSettingKeyProperty =
            DependencyProperty.Register("StopSettingKey", typeof(Key), typeof(KeyBindingUserControl), new PropertyMetadata(Key.Escape));

        public Key StopSettingKey
        {
            get { return (Key)GetValue(StopSettingKeyProperty); }
            set { SetValue(StopSettingKeyProperty, value); }
        }

        #endregion

        private void SetBindingProperties(Key triggerKey, ModifierKeys modifierKeys)
        {
            TriggerKey = triggerKey;
            KeyCombination = KeyCombination.TriggeredBy(FormsWpfKeysHelper.FormsKeyFromWpfKey(TriggerKey));
            if (IsModifiersEnabled)
            {
                Modifiers = modifierKeys;
                KeyCombination = KeyCombination.With(FormsWpfKeysHelper.FormsModifiersFromWpfModifiers(Modifiers));
            }
            else
            {
                Modifiers = ModifierKeys.None;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEnabled || !IsFocused || !IsSettingKeys) return;

            if (e.Key == Key.Tab) return;
            if (e.Key == StopSettingKey)
            {
                IsSettingKeys = false;
                return;
            }

            if (e.Key == NoneKey)
            {
                KeyCombination = KeyCombination.TriggeredBy(System.Windows.Forms.Keys.None);
            }
            else
            {
                var triggerKeyWithMods = FormsWpfKeysHelper.WpfKeysFromWpfKeyEventArgs(e);

                SetBindingProperties(triggerKeyWithMods.TriggerKey, triggerKeyWithMods.ModifierKeys);
            }

        }

        private void KeyBindingUserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchSettingState();

            e.Handled = true;
        }

        private void SwitchSettingState()
        {
            if (!IsSettingKeys)
            {
                Focus();
                IsSettingKeys = true;
            }
            else IsSettingKeys = false;
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void KeyBindingUserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            SwitchSettingState();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            IsSettingKeys = false;
            base.OnLostFocus(e);
        }

    }
}
