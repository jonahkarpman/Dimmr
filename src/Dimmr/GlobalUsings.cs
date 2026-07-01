// Both WPF and WinForms are enabled, which makes some type names ambiguous.
// Pin the ones we use to their WPF meaning across the whole project.
global using Application = System.Windows.Application;
