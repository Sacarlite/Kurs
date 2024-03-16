using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AproximateMetods;
using Data;
using DesctopPart;
using DesktopPart.Service;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ServerBroadcast;
using SkiaSharp;

namespace DesktopPart.ViewModel;

public class MainViewModel : BaseVievModel
{
    private readonly ObservableCollection<ObservablePoint> ConcentrationLine;
    private readonly ObservableCollection<ObservablePoint> ConcentrationPoints;
    private readonly IExelService _excel;
    private readonly ObservableCollection<ObservablePoint> LevelLine;
    private readonly ObservableCollection<ObservablePoint> LevelPoints;
    private ManyParameters _approximateParameters;
    private ObservableCollection<InputParameters> _userParameters;

    public MainViewModel(IExelService excel)
    {
        this._excel = excel;
        Methods = new ObservableCollection<object>
        {
            new Linear(),
            new Parabola()
        };

        LevelPoints = new ObservableCollection<ObservablePoint>();
        ConcentrationPoints = new ObservableCollection<ObservablePoint>();
        LevelLine = new ObservableCollection<ObservablePoint>();
        ConcentrationLine = new ObservableCollection<ObservablePoint>();
        UserParameters = new ObservableCollection<InputParameters>();


        LevelSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservablePoint>
            {
                Values = LevelLine,
                Stroke = new SolidColorPaint(new SKColor(154, 205, 50), 2),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 4
            },
            new ScatterSeries<ObservablePoint>
            {
                Values = LevelPoints,
                Stroke = new SolidColorPaint(new SKColor(120, 120, 120), 2),
                Fill = default,
                GeometrySize = 4
            }
        };

        ConcentrationSeries = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservablePoint>
            {
                Values = ConcentrationLine,
                Stroke = new SolidColorPaint(new SKColor(154, 205, 50), 2),
                Fill = null,
                GeometrySize = 0,
                LineSmoothness = 4
            },
            new ScatterSeries<ObservablePoint>
            {
                Values = ConcentrationPoints,
                Stroke = new SolidColorPaint(new SKColor(120, 120, 120), 2),
                Fill = default,
                GeometrySize = 4
            }
        };
    }

    public ObservableCollection<ISeries> LeftGraph { get; set; }

    public ObservableCollection<ISeries> RightGraph { get; set; }
    public ObservableCollection<object> Methods { get; set; }

    public object? SelectedMethod { get; set; }


    public ObservableCollection<InputParameters> UserParameters
    {
        get => _userParameters;
        set
        {
            if (_userParameters != value)
            {
                _userParameters = value;
                OnPropertyChanged();
            }
        }
    }

    public Axis[] XExpenditure { get; set; } =
    {
        new()
        {
            Name = "Расход, кг/сек",
            NameTextSize = 11
        }
    };

    public Axis[] YLevel { get; set; } =
    {
        new()
        {
            Name = "Уровень, ДМ",
            NameTextSize = 11
        }
    };


    public Axis[] YConcentration { get; set; } =
    {
        new()
        {
            Name = "Концентрация, мг/м^3",
            NameTextSize = 11
        }
    };

    public ObservableCollection<ISeries> LevelSeries { get; set; }
    public ObservableCollection<ISeries> ConcentrationSeries { get; set; }

    public ICommand DrawApproximation
    {
        get
        {
            return new DelegateCommand(() =>
                {
                    try
                    {
                        if (SelectedMethod==null || UserParameters == null)
                        {
                            throw new Exception("Обработка невозможна т.к не введены данные или не выбран метод");
                        }

                        if (UserParameters.Count < 2)
                        {
                            throw new Exception("Неверно введены данные");
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }

                    TcpConnect();
                }
            );
        }
    }
    public ICommand SaveExportDataCommand
    {
        get
        {
            return new DelegateCommand(() =>
                {
                    try
                    {
                        if (_approximateParameters == null || UserParameters == null)
                        {
                            throw new Exception("Сохранение невозможно т.к не введены данные");
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    }
                    _excel.ExportApproximationToExel(new ManyParameters(UserParameters), _approximateParameters);
                    
                }
            );
        }
    }
    public ICommand SaveInputDataCommand
    {
        get
        {
            return new DelegateCommand(() =>
                {
                    try
                    {
                        if ( UserParameters == null)
                        {
                            throw new Exception("Сохранение невозможно т.к не введены данные");
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        return;
                    } 
                    _excel.ExportInputDataToExel(new ManyParameters(UserParameters));
                }
            );
        }
    }


    public ICommand Clear
    {
        get { return new DelegateCommand(() => { ClearPlot(); }); }
    }

    public ICommand LoadData
    {
        get
        {
            return new DelegateCommand(() =>
                {
                    UserParameters = _excel.ImportFromExcel().ToObservableCollection();
                    if (UserParameters.Count != 0) PointValue(UserParameters);
                }
            );
        }
    }


    private void PointValue(ObservableCollection<InputParameters> _newPoints)
    {
        foreach (var point in _newPoints)
        {
            ConcentrationPoints.Add(new ObservablePoint(point.Expenditure, point.Concentration));
            LevelPoints.Add(new ObservablePoint(point.Expenditure, point.Level));
        }
    }

    private void LineValue(ManyParameters _newPoints)
    {
        for (var i = 0; i < _newPoints.Expenditures.Count; i++)
        {
            ConcentrationLine.Add(new ObservablePoint(_newPoints.Expenditures[i], _newPoints.Concentrations[i]));
            LevelLine.Add(new ObservablePoint(_newPoints.Expenditures[i], _newPoints.Levels[i]));
        }
    }


    private void TcpConnect()
    {
        if (UserParameters.Count >= 2)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(Desctop.Default.Adress, Desctop.Default.Port);
            var stream = tcpClient.GetStream();
            var transfers = new ServerTransfer();
            // буфер для входящих данных
            var response = new List<byte>();
            var bytesRead = 10; // для считывания байтов из потока

            // считываем строку в массив байтов
            // при отправке добавляем маркер завершения сообщения - \n


            var data = Encoding.UTF8.GetBytes(transfers.AppEncoder(new ManyParameters(UserParameters), SelectedMethod,
                Desctop.Default.Step) + '\n');
            // отправляем данные
            stream.WriteAsync(data);
            // считываем данные до конечного символа
            while ((bytesRead = stream.ReadByte()) != '\n')
                // добавляем в буфер
                response.Add((byte)bytesRead);
            try
            {
                var translation = Encoding.UTF8.GetString(response.ToArray());
                var newParameters = transfers.AppDecoder(translation);
                stream.WriteAsync(Encoding.UTF8.GetBytes("END\n"));
                LineValue(newParameters);
                _approximateParameters = newParameters;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            response.Clear();
            stream.Close();
        }
    }

    private void ClearPlot()
    {
        UserParameters.Clear();
        LevelPoints.Clear();
        ConcentrationPoints.Clear();
        ConcentrationLine.Clear();
        LevelLine.Clear();
    }
}