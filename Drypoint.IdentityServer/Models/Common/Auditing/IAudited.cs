using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.IdentityServer.Models.Common.Auditing
{
    public interface IAudited<TPrimaryKey> : ICreationAudited<TPrimaryKey>, IModificationAudited<TPrimaryKey>
    {

       
    }
}
