using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.IdentityServer.Hosting.Models.Common.Auditing
{
    public interface IFullAudited<TPrimaryKey> : IAudited<TPrimaryKey>, IDeletionAudited<TPrimaryKey>
    {

    }

}
