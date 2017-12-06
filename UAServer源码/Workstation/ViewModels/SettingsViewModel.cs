// Copyright (c) 2014 Converter Systems LLC

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using MahApps.Metro;

namespace ConverterSystems.Workstation.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public static List<Accent> AccentColors = ThemeManager.DefaultAccents.ToList();
        public static List<Theme> Themes = Enum.GetValues(typeof (Theme)).OfType<Theme>().ToList();

        private Accent _selectedAccent;
        private Theme _selectedTheme;

        public SettingsViewModel()
        {
            _selectedTheme = Theme.Light;
            _selectedAccent = AccentColors[0];

            var current = ThemeManager.DetectTheme(Application.Current);
            if (current != null)
            {
                _selectedTheme = current.Item1;
                _selectedAccent = current.Item2;
            }
        }

        public Theme SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                NotifyPropertyChanged();
                ThemeManager.ChangeTheme(Application.Current, _selectedAccent, _selectedTheme);
            }
        }

        public Accent SelectedAccent
        {
            get { return _selectedAccent; }
            set
            {
                _selectedAccent = value;
                NotifyPropertyChanged();
                ThemeManager.ChangeTheme(Application.Current, _selectedAccent, _selectedTheme);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}