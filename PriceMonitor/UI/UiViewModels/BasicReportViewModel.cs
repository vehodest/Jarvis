﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Entity.DataTypes;
using EveCentralProvider;
using EveCentralProvider.Types;
using Helpers;

namespace PriceMonitor.UI.UiViewModels
{
	public class BasicReportViewModel : BaseViewModel
	{
		private readonly string _loadMessage = "Loading...";

		public BasicReportViewModel()
		{
			IsAggregateInfoOpen = false;

			Report = new BasicReportData()
			{
				Item = new GameObject()
				{
					Name = _loadMessage
				}
			};
		}

		private BasicReportData _report;
		public BasicReportData Report
		{
			get => _report;
			set
			{
				_report = value;
				NotifyPropertyChanged();
			}
		}

		public void AssignReport(BasicReportData report)
		{
			Report = report;
		}

		private DataTable _aggregateList;
		public DataTable AggregateList
		{
			get => _aggregateList;
			set
			{
				_aggregateList = value;
				NotifyPropertyChanged();
			}
		}

		private RelayCommand _showAggregateInfoCmd;
		public RelayCommand ShowAggregateInfoCmd
		{
			get
			{
				return _showAggregateInfoCmd ?? (_showAggregateInfoCmd = new RelayCommand(p => AggregateInfo()));
			}
		}

		private bool _isAggregateInfoOpen;
		public bool IsAggregateInfoOpen
		{
			get => _isAggregateInfoOpen;
			set
			{
				_isAggregateInfoOpen = value;
				NotifyPropertyChanged();
			}
		}

		private void AggregateInfo()
		{
			if (!IsAggregateInfoOpen)
			{
				if (AggregateList == null)
				{
					Task.Run(async () =>
					{
						DataTable table = new DataTable();

						table.Columns.Add("info");

						var aggregateStats = new List<AggreateInfoStat>();

						table.Columns.Add(Report.BuyStation.Name.Substring(0, Report.BuyStation.Name.IndexOf(' ')));
						var buyStationAggregate = await Services.Instance.AggregateInfoAsync(Report.Item.TypeId, Report.BuyStation.RegionId);
						aggregateStats.Add(buyStationAggregate.Items.First().sell);

						table.Columns.Add(Report.SellStation.Name.Substring(0, Report.SellStation.Name.IndexOf(' ')));
						var sellStationAggregate = await Services.Instance.AggregateInfoAsync(Report.Item.TypeId, Report.SellStation.RegionId);
						aggregateStats.Add(sellStationAggregate.Items.First().sell);

						var stats = typeof(AggreateInfoStat).GetProperties();
						foreach (var stat in stats)
						{
							var nextRow = table.NewRow();

							int index = 0;
							nextRow[index] = stat.Name;

							foreach (var hubStat in aggregateStats)
							{
								index++;
								nextRow[index] = typeof(AggreateInfoStat).GetProperty(stat.Name).GetValue(hubStat, null);
							}
							table.Rows.Add(nextRow);
						}

						Application.Current.Dispatcher.Invoke(() =>
						{
							AggregateList = table;
						});
					});
				}
				IsAggregateInfoOpen = true;
			}
			else
			{
				IsAggregateInfoOpen = false;
			}
		}
	}
}
