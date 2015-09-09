﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;


    namespace ErrorHedging
    {

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
  
    class MainWindowViewModel : BindableBase
    {
        private string _nomAction;  
        public string nomAction
        {
            get { return _nomAction; }
            set { SetProperty(ref _nomAction, value); ;  }
        }


        private string _maturite;
        public string maturite
        {
            get { return _maturite; }
            set { SetProperty(ref _maturite, value); }
        }


        private string _strikePrice;
        public string strikePrice
        {
            get { return _strikePrice; }
            set { SetProperty(ref _strikePrice, value); }
        }


        private string _dateDebut;
        public string dateDebut
        {
            get { return _dateDebut; }
            set { SetProperty(ref _dateDebut, value); }
        }


        private string _typeDonnees;
        public string typeDonnees
        {
            get { return _typeDonnees; }
            set { SetProperty(ref _typeDonnees, value); }
        }


        private string _dureeEstimation;
        public string dureeEstimation
        {
            get { return _dureeEstimation; }
            set { SetProperty(ref _dureeEstimation, value); }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("doit afficher le payoff de l'option et la valeur du portefeuille de couverture correspondant");
        }

    }
}  





