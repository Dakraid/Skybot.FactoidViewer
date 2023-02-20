// Skybot.FactoidViewer - FactoidsService.cs
// Created on 2023.02.17
// Last modified at 2023.02.17 17:21

#region
#endregion

#region
using Skybot.FactoidViewer.Models;
#endregion

namespace Skybot.FactoidViewer.Services;

public class FactoidsService
{
    private readonly FactoidsContext _context = new();

    public Task<IQueryable<Factoid>> GetAllAsync()
    {
        return Task.FromResult(_context.Factoids.OrderByDescending(p => p.Key).AsQueryable());
    }
}
