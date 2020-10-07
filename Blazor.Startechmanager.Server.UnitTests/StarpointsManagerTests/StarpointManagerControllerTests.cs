using Blazor.Startechmanager.Server.Controllers;
using Blazor.Startechmanager.Server.Models;
using Blazor.Startechmanager.Shared.Models;
using Blazor.Startechmanager.Shared.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blazor.Startechmanager.Server.UnitTests
{
    public class StarpointManagerControllerTests : BaseTestsWithDbContext<StarPointsManagerController>
    {

        public Mock<UserManager<ApplicationUser>> UserManager { get; set; }

        public Mock<IAuthorizationService> AuthorizationService { get; set; }


        protected void SetUser(ApplicationUser user)
        {
            UserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.Factory.StartNew(() => user));
        }

        protected void AuthorizeLeader()
        {
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),It.IsAny<object>(), It.IsIn(StartechPolicyHelper.AllStartechLeader)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));
            var policy = StartechPolicyHelper.GetPolicyName(Startechs.Dotnet, MustBeLeader: true);
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn( policy)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Success()));

            foreach(var startech in Enum.GetValues(typeof(Startechs)).Cast<Startechs>().Where(x => x != Startechs.Dotnet))
            {
                var ThisLoopPolicy = StartechPolicyHelper.GetPolicyName(startech, MustBeLeader: true);
                AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(ThisLoopPolicy )))
                    .Returns(Task.Factory.StartNew(() => AuthorizationResult.Failed()));                
            }
        }

        protected void UnauthorizeMember()
        {
            AuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsIn(StartechPolicyHelper.AllStartechLeader)))
                .Returns(Task.Factory.StartNew(() => AuthorizationResult.Failed()));
        }

        protected readonly StarpointsType BlogArticle = new StarpointsType
        {
            Id = 1,
            TypeName = "Blog Article",
            NumberOfPoint = 15,
            IsActive = true
        };

        protected readonly StarpointsType Course = new StarpointsType
        {
            Id = 2,
            TypeName = "Course",
            NumberOfPoint = 150,
            IsActive = true
        };

        protected readonly StarpointsType Presentation = new StarpointsType
        {
            Id = 3,
            TypeName = "Presentation",
            NumberOfPoint = 80,
            IsActive = true
        };

        protected readonly StarpointsType NonActiveType = new StarpointsType
        {
            Id = 4,
            TypeName = "Obsolete",
            NumberOfPoint = 999,
            IsActive = false
        };

        protected readonly ApplicationUser LeaderDotnet = new ApplicationUser
        {
            Id = 3,
            UserName = "Leader dotnet",
            Startechs = new List<MappingStartechUser> { 
                    new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = true, ApplicationUserId = 3 },
                     new MappingStartechUser { Startech = Startechs.Java, IsLeader = false, ApplicationUserId = 3 }}

        };

        protected readonly ApplicationUser MemberDotnet = new ApplicationUser
        {
            Id = 5,
            UserName = "Member dotnet",
            Startechs = new List<MappingStartechUser> { new MappingStartechUser { Startech = Startechs.Dotnet, IsLeader = false, ApplicationUserId = 3 },
                                    new MappingStartechUser { Startech = Startechs.Java, IsLeader = false, ApplicationUserId = 3 }
            }

        };

        protected readonly ApplicationUser User = new ApplicationUser
        {
            Id = 7,
            UserName = "User"
        };

        [SetUp]
        public void SetupValues()
        {
            DbContext.Add(BlogArticle);
            DbContext.Add(Course);
            DbContext.Add(Presentation);
            DbContext.Add(NonActiveType);


            DbContext.Add(LeaderDotnet);
            DbContext.Add(MemberDotnet);
            DbContext.Add(User);


            DbContext.Add(new StarpointsItem
            {
                Id = 1,
                ApplicationUserId = LeaderDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = BlogArticle.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 1",
                ValidationState = ValidationState.Validated
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 2,
                ApplicationUserId = LeaderDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = null,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 2",
                ValidationState = ValidationState.Validated
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 3,
                ApplicationUserId = LeaderDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = Presentation.Id,
                Startech = Startechs.Java,
                TextJustification = "justification 3",
                ValidationState = ValidationState.InStudy
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 4,
                ApplicationUserId = LeaderDotnet.Id,
                Date = DateTime.Now.AddYears(-5),
                NumberOfPoints = 15,
                StarpointsTypeId = BlogArticle.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 4",
                ValidationState = ValidationState.Validated
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 5,
                ApplicationUserId = LeaderDotnet.Id,
                Date = DateTime.Now.AddYears(-12),
                NumberOfPoints = 15,
                StarpointsTypeId = BlogArticle.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 4",
                ValidationState = ValidationState.Validated
            });

            DbContext.Add(new StarpointsItem
            {
                Id = 6,
                ApplicationUserId = MemberDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = BlogArticle.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 1",
                ValidationState = ValidationState.Validated
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 7,
                ApplicationUserId = MemberDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = BlogArticle.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 2",
                ValidationState = ValidationState.InStudy
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 8,
                ApplicationUserId = MemberDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = Presentation.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 3",
                ValidationState = ValidationState.Refused
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 9,
                ApplicationUserId = MemberDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = Presentation.Id,
                Startech = Startechs.Java,
                TextJustification = "justification 4",
                ValidationState = ValidationState.InStudy
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 10,
                ApplicationUserId = MemberDotnet.Id,
                Date = DateTime.Now.AddDays(-10),
                NumberOfPoints = 15,
                StarpointsTypeId = Presentation.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 3",
                ValidationState = ValidationState.InStudy
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 11,
                ApplicationUserId = MemberDotnet.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = Presentation.Id,
                Startech = Startechs.Agile,
                TextJustification = "justification 4",
                ValidationState = ValidationState.InStudy
            });
            DbContext.Add(new StarpointsItem
            {
                Id = 12,
                ApplicationUserId = User.Id,
                Date = DateTime.Now,
                NumberOfPoints = 15,
                StarpointsTypeId = BlogArticle.Id,
                Startech = Startechs.Dotnet,
                TextJustification = "justification 8",
                ValidationState = ValidationState.InStudy
            });



            DbContext.SaveChanges();
        }

        protected override void SetMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            UserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            ServiceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor() { HttpContext = new DefaultHttpContext() });
        }

    }
}
