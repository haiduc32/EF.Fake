﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	public interface IEntityStructureProvider
	{
		List<NavigationPropertyInfo> NavigationProperties { get; }
		List<EntityKeysInfo> EntityKeys { get; }
	}
}
