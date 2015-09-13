using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Windows.Threading;

namespace ErrorHedging
{
    class MainWindowViewModel : BindableBase
    {

        private ComponentInfo selectedComponent;
        public ObservableCollection<ComponentInfo> ComponentInfoList { get; private set; }
        public ICommand ClickCommand { get; private set; }
        public PlotModel MyModel { get; private set; }
        public PlotController control = new PlotController();



        public ComponentInfo SelectedComponent
        {
            get { return selectedComponent; }
            set
            {
                SetProperty(ref selectedComponent, value);
                Console.WriteLine("Component " + selectedComponent.Name + " is selected");
            }
        }


        public MainWindowViewModel()
        {

            this.typeDonnees = "Simulées";
            this._typeOption = "Basket Option";
            this.maturite = new DateTime(2015, 01, 01);
            this.dateDebut = new DateTime(2014, 01, 01);

            var listComp = GetComponents();
            ComponentInfoList = new ObservableCollection<ComponentInfo>(listComp);
            MyModel = new PlotModel();
            SetUpModel(0, 10, new DateTime(2010, 05, 04));
            ClickCommand = new DelegateCommand(ExtractComponents);

        }



        private void SetUpModel(double minValue, double maxValue, DateTime date1)
        {

            MyModel.LegendTitle = "Legend";
            MyModel.LegendOrientation = LegendOrientation.Horizontal;
            MyModel.LegendPlacement = LegendPlacement.Outside;
            MyModel.LegendPosition = LegendPosition.TopRight;
            MyModel.LegendBackground = OxyColor.FromAColor(100, OxyColors.White);
            MyModel.LegendBorder = OxyColors.Black;

            var dateAxis = new OxyPlot.Axes.DateTimeAxis(AxisPosition.Bottom, "Date", "dd/MM/yy HH:mm") { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80, Minimum = DateTimeAxis.ToDouble(date1) };

            MyModel.Axes.Add(dateAxis);

            var valueAxis = new OxyPlot.Axes.LinearAxis(AxisPosition.Left, minValue, maxValue) { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            MyModel.Axes.Add(valueAxis);
        }


        private void ExtractComponents()
        {
            // On test les différents paramètres, si ils ne sont pas bons on fait apparaitre un pop-up
            double sommePoids = 0.0;
            int nbAction = 0;
            foreach (var comp in ComponentInfoList)
            {
                if (comp.IsSelected)
                {
                    if (comp.Poids < 0.0)
                    {
                        System.Windows.Forms.MessageBox.Show("ERREUR : Les poids doivent être positif");
                        return;
                    }
                    sommePoids += comp.Poids;
                    nbAction += 1;
                }
            }
            if (sommePoids != 1.0)
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : La somme des poids doit être égale à 1 ");
                return;
            }

            PricingLibrary.FinancialProducts.Share[] tabShare = new PricingLibrary.FinancialProducts.Share[nbAction];
            double[] tabWeight = new double[nbAction];

            if (tailleFenetre < 0)
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : La taille de la fenetre doit être superieure à 0 ");
                return;
            }

            if (maturite.Subtract(dateDebut).Days < tailleFenetre)
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : La taille de la fenetre ne doit pas être plus grande que l'écart entre la maturite et la date de début ");
                return;
            }

            if (tailleFenetre < nbAction)
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : La taille de la fenetre doit être un entier au minimum égale au nombre de sous jacent ");
                return;
            }
            if ((1 < nbAction)&&(typeOption.Equals("Vanilla Call")))
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : Un Vanilla Call ne peut avoir qu'un sous-jacent ");
                return;
            }


            // On utilise maintenant nb action comme index
            nbAction = 0;
            foreach (var comp in ComponentInfoList)
            {
                if (comp.IsSelected)
                {
                    PricingLibrary.FinancialProducts.Share share = new PricingLibrary.FinancialProducts.Share(comp.Name, comp.Name);
                    tabShare[nbAction] = share;
                    tabWeight[nbAction] = comp.Poids;
                    nbAction += 1;
                }
            }



            if (strikePrice < 0)
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : Le strike doit être positif");
                return;
            }

            TimeSpan diff = maturite.Subtract(dateDebut);
            if (diff.Days < 0)
            {
                System.Windows.Forms.MessageBox.Show("ERREUR : La date de debut doit etre avant la maturite ");
                return;
            }


            if (!typeDonnees.Equals("Simulées") && !typeDonnees.Equals("Historiques"))
            {
                System.Windows.Forms.MessageBox.Show("ERREUR Type de données : Choisir l'une des deux possibilités ");
                return;
            }
            bool simule;
            if (typeDonnees.Equals("Simulées")) {
                simule = true;
            } else {
                simule = false;
            }

            PricingLibrary.FinancialProducts.IOption option;
            if (typeOption.Equals("Vanilla Call"))
            {
                option = new PricingLibrary.FinancialProducts.VanillaCall("call", tabShare, maturite, strikePrice);
            }
            else
            {
                option = new PricingLibrary.FinancialProducts.BasketOption("basket", tabShare, tabWeight, maturite, strikePrice);
            }

            ////////////////////////////////////////////////////////////////////////////

            // On calcule le portefeuilles de couverture de l'option
            OptionManager optionCompute = new OptionManager(option,dateDebut,maturite,tailleFenetre,simule);
            ComputeResults.computeResults(optionCompute);

            // On trace les graphes
            LineSeries courbe = tabToSeries(optionCompute.OptionPrice, optionCompute.dateTime);
            LineSeries courbe2 = tabToSeries(optionCompute.HedgingPortfolioValue, optionCompute.dateTime);

            this.MyModel.Series.Clear();
            this.MyModel.Axes.Clear();

            double min = Math.Min(optionCompute.OptionPrice.Min(),optionCompute.HedgingPortfolioValue.Min())-0.1;
            double max = Math.Max(optionCompute.OptionPrice.Max(), optionCompute.HedgingPortfolioValue.Max())+0.1;
            SetUpModel(min, max, optionCompute.dateTime.Min());

            courbe.Title = "Prix de l'option";
            courbe2.Title = "Valeur Portefeuille de Couverture";
            this.MyModel.Series.Add(courbe);
            this.MyModel.Series.Add(courbe2);

            // On met à jour les graphes
            this.MyModel.InvalidatePlot(true);
            this.MyModel.PlotView.InvalidatePlot(true);

        }



        private List<ComponentInfo> GetComponents()
        {
            return new List<ComponentInfo>()
            {
                new ComponentInfo() {Name = "ALO FP", IsSelected = false},
                new ComponentInfo() {Name = "EDF FP", IsSelected = false},
                new ComponentInfo() {Name = "AIR FP", IsSelected = false},
                new ComponentInfo() {Name = "AI FP", IsSelected = false},
                new ComponentInfo() {Name = "CA FP", IsSelected = false},
                new ComponentInfo() {Name = "EI FP", IsSelected = false},
                new ComponentInfo() {Name = "BNP FP", IsSelected = false},
                new ComponentInfo() {Name = "GLE FP", IsSelected = false},
                new ComponentInfo() {Name = "AC FP", IsSelected = false},
                new ComponentInfo() {Name = "BN FP", IsSelected = false},
                new ComponentInfo() {Name = "CS FP", IsSelected = false},
                new ComponentInfo() {Name = "EN FP", IsSelected = false},
                new ComponentInfo() {Name = "ACA FP", IsSelected = false},
                new ComponentInfo() {Name = "CAP FP", IsSelected = false}
            };
        }



        private DateTime _maturite;
        public DateTime maturite
        {
            get { return _maturite; }
            set { SetProperty(ref _maturite, value); }
        }



        private double _strikePrice;
        public double strikePrice
        {
            get { return _strikePrice; }
            set { SetProperty(ref _strikePrice, value); }
        }

        private int _tailleFenetre;
        public int tailleFenetre
        {
            get { return _tailleFenetre; }
            set { SetProperty(ref _tailleFenetre, value); }
        }


        private DateTime _dateDebut;
        public DateTime dateDebut
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

        private string _typeOption;
        public string typeOption
        {
            get { return _typeOption; }
            set { SetProperty(ref _typeOption, value); }
        }

        

        public LineSeries tabToSeries(List<double> tabValue, List<DateTime> dateTab)
        {
            int i = 0;
            OxyPlot.Series.LineSeries courbe = new OxyPlot.Series.LineSeries();
            foreach (double value in tabValue)
            {
                DataPoint pointTmp = DateTimeAxis.CreateDataPoint(dateTab[i], value);
                courbe.Points.Add(pointTmp);
                i++;
            }
            return courbe;
        }

    }
}




