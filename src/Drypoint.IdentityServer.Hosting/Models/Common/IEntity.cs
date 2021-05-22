using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.IdentityServer.Hosting.Models.Common
{
    public interface IEntity<TPrimaryKey>
    {
        TPrimaryKey Id { get; set; }

        bool IsTransient();
    }
}
