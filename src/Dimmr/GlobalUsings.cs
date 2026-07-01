// Both WPF and WinForms are enabled, which makes some type names ambiguous.
// Pin the ones we use to their WPF meaning across the whole project.
global using Application = System.Windows.Application;
global using Button = System.Windows.Controls.Button;
global using CheckBox = System.Windows.Controls.CheckBox;
global using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
global using ToggleButton = System.Windows.Controls.Primitives.ToggleButton;
