﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Shell.Interop;
using SmartCmdArgs.Helper;
using SmartCmdArgs.Model;

namespace SmartCmdArgs.ViewModel
{
    public class ToolWindowViewModel : PropertyChangedBase
    {
        public ListViewModel CommandlineArguments { get; private set; }

        private string startupProject;
        public string StartupProject
        {
            get { return startupProject; }
            private set { startupProject = value; OnNotifyPropertyChanged(); }
        }

        private RelayCommand addEntryCommand;
        public RelayCommand AddEntryCommand { get { return addEntryCommand; } }


        private RelayCommand<IList> removeEntriesCommand;
        public RelayCommand<IList> RemoveEntriesCommand { get { return removeEntriesCommand; } }


        private RelayCommand<IList> moveEntriesUpCommand;
        public RelayCommand<IList> MoveEntriesUpCommand { get { return moveEntriesUpCommand; } }


        private RelayCommand<IList> moveEntriesDownCommand;
        public RelayCommand<IList> MoveEntriesDownCommand { get { return moveEntriesDownCommand; } }

        public ToolWindowViewModel()
        {
            this.CommandlineArguments = new ListViewModel();
            this.StartupProject = CmdArgStorage.Instance.StartupProject;

            CmdArgStorage.Instance.StartupProjectChanged += Instance_StartupProjectChanged;

            addEntryCommand = new RelayCommand(
                () => {
                    var newItem = CmdArgStorage.Instance.AddEntry(command: "", enabled: true);
                    CommandlineArguments.AddCmdArgStoreEntry(newItem);
                }, canExecute: _ =>
                {
                    return this.StartupProject != null;
                });

            removeEntriesCommand = new RelayCommand<IList>(
               items => {
                   if (items != null && items.Count != 0)
                   {
                       foreach (var item in items.Cast<CmdArgItem>())
                       {
                           CmdArgStorage.Instance.RemoveEntryById(item.Id);
                           CommandlineArguments.RemoveById(item.Id);
                       }
                   }
               }, canExecute: _ =>
               {
                   return this.StartupProject != null;
               });

            moveEntriesUpCommand = new RelayCommand<IList>(
               items => {
                   if (items != null && items.Count != 0)
                   {
                       foreach (var item in items.Cast<CmdArgItem>())
                       {
                           CmdArgStorage.Instance.MoveEntryUp(item.Id);
                           CommandlineArguments.MoveEntryUp(item.Id);
                       }
                   }
               }, canExecute: _ =>
               {
                   return this.StartupProject != null;
               });

            moveEntriesDownCommand = new RelayCommand<IList>(
               items =>
               {
                   if (items != null && items.Count != 0)
                   {
                       foreach (var item in items.Cast<CmdArgItem>())
                       {
                           CmdArgStorage.Instance.MoveEntryDown(item.Id);
                           CommandlineArguments.MoveEntryDown(item.Id);
                       }
                   }
               }, canExecute: _ =>
               {
                   return this.StartupProject != null;
               });
        }

        public void UpdateView()
        {
            this.StartupProject = CmdArgStorage.Instance.StartupProject;

            if (StartupProject != null)
            {
                this.CommandlineArguments.SetListItems(CmdArgStorage.Instance.StartupProjectEntries);
            }
        }

        private void Instance_StartupProjectChanged(object sender, EventArgs e)
        {
            UpdateView();
        }
    }
}