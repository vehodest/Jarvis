﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.DataTypes;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;

namespace Entity
{
	public sealed class EntityService
	{
		public static EntityService Instance { get; } = new EntityService();
		private EntityService() { }

		public async Task<List<SolarSystem>> RequestSystemsByRegionAsync(int regionId)
		{
			using (var ctx = new sysEntities())
			{
				var list = await ctx.eve_map_solarsystems
					.Where(t => t.region_id == regionId)
					.OrderBy(t => t.solarsystem_name)
					.Select(t => new SolarSystem()
					{
						Name = t.solarsystem_name,
						RegionId = (int)t.region_id,
						SystemId = t.solarsystem_id
					})
					.ToListAsync();

				return list;
			}
		}

		public async Task<List<Station>> RequestStationsBySystemAsync(int systemId)
		{
			using (var ctx = new sysEntities())
			{
				var list = await ctx.eve_sta_stations
					.Where(t => t.solarsystem_id == systemId)
					.OrderBy(t => t.station_name)
					.Select(t => new Station()
					{
						Name = t.station_name,
						RegionId = t.region_id,
						StationId = t.station_id,
						SystemId = t.solarsystem_id
					})
					.ToListAsync();

				return list;
			}
		}

		public async Task<List<Station>> RequestStationsByRegionAsync(int regionId)
		{
			using (var ctx = new sysEntities())
			{
				var list = await ctx.eve_sta_stations
					.Where(t => t.region_id == regionId)
					.OrderBy(t => t.station_name)
					.Select(t => new Station()
					{
						Name = t.station_name,
						RegionId = t.region_id,
						StationId = t.station_id
					})
					.ToListAsync();

				return list;
			}
		}

		public async Task<List<Region>> RequestRegionsAsync()
		{
			using (var ctx = new sysEntities())
			{
				// except Unknown systems by filtering id
				var list = await ctx.eve_map_regions
					.Where(t => t.region_id < 11000000)
					.OrderBy(t => t.region_name)
					.Select(t => new Region()
					{
						Name = t.region_name,
						RegionId = t.region_id
					})
					.ToListAsync();

				return list;
			}
		}

		private Expression<Func<eve_inv_types, bool>> _itemFilterFunc = t => true;
		private readonly object _syncFilterFunc = new object();
		public Expression<Func<eve_inv_types, bool>> FilterFunc
		{
			get
			{
				lock (_syncFilterFunc)
				{
					return _itemFilterFunc;
				}
			}
			set
			{
				lock (_syncFilterFunc)
				{
					_itemFilterFunc = value;
				}
			}
		}

		public IEnumerable<ObjectsNode> RequestObjectNodes(int level = 0)
		{
			return DoRequestChain(level);
		}

		private IEnumerable<ObjectsNode> DoRequestChain(int level)
		{
			using (var ctx = new sysEntities())
			{
				var list = ctx.eve_inv_marketgroups
					.Where(t => t.parentgroup_id == level)
					.Select(t => new ObjectsNode()
					{
						Object = new GameObject()
						{
							Name = t.marketgroup_name,
							MarketGroupId = t.marketgroup_id,
							TypeId = 0
						}
					})
					.ToList();

				if (!list.Any())
				{
					list = ctx.eve_inv_types
						.Where(t => t.marketgroup_id == level)
						.AsExpandable()
						.Where(FilterFunc)
						.Select(t => new ObjectsNode()
						{
							Object = new GameObject()
							{
								Name = t.name,
								MarketGroupId = (int)t.marketgroup_id,
								TypeId = t.type_id
							}
						})
						.ToList();

					foreach (var item in list)
					{
						yield return item;
					}
				}
				else
				{
					foreach (var item in list)
					{
						item.SubObjects = DoRequestChain(item.Object.MarketGroupId);
						yield return item;
					}
				}
			}
		}

		public async Task<List<GameObject>> RequestObjectsAsync(int marketGroudpId)
		{
			using (var ctx = new sysEntities())
			{
				var list = await ctx.eve_inv_types.Where(t => t.marketgroup_id == marketGroudpId)
					.Select(t => new GameObject()
					{
						Name = t.name,
						MarketGroupId = (int)t.marketgroup_id,
						TypeId = t.type_id
					})
					.ToListAsync();

				return list;
			}
		}
	}
}
