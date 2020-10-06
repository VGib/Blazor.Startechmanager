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

        #region UpdateValidationStatus

        [Test]
        public async Task UpdateValidationStatus_should_be_used_only_by_this_startech_owner()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_should_not_be_used_by_a_startech_leader_from_an_other_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_should_save_the_new_validation_status()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_not_updated_from_Validation_no_starpoints_should_be_added_to_the_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_updated_to_validated_the_starpoints_should_be_added_to_the_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_if_validation_status_is_removed_the_starpoint_should_be_removed_from_the_user()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateValidationStatus_validating_the_same_status_should_be_an_error()
        {
            throw new NotImplementedException("to do");
        }

        #endregion

        #region UpdateStarpoints

        [Test]
        public async Task UpdateStarpoints_only_valid_starpoints_should_be_updated()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_url_or_description()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_only_update_his_starpoint_if_they_are_not_validated()
        {
            throw new NotImplementedException("to do");
        }


        [Test]
        public async Task UpdateStarpoints_current_user_can_update_his_starpoint_even_if_his_not_startech_memeber_anymore()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_can_not_modify_starpoint_validation_status()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_current_user_can_not_modify_starpoint_number()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_startech_leader_can_modify_starpoints__number_only_if_starpoints_have_not_been_validated()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_users_starpoint_can_not_be_modifyed()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_starpoint_date_can_not_be_modifyed()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_updates_the_starpoints_in_database()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_only_startech_leader_can_modify_the_starpoint_type()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task UpdateStarpoints_starpoint_type_can_be_modifyed_only_on_InValidation_state()
        {
            throw new NotImplementedException("to do");
        }

        #endregion

        #region CancelStarpoints

        [Test]
        public async Task CancelStarpoints_only_inValidation_starpoints_can_be_cancelled()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_his_starpoints()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_cancel_his_starpoint_even_if_is_not_member_of_the_startech()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_current_user_can_only_cancel_from_other_user_for_startech_where_is_member_of()
        {
            throw new NotImplementedException("to do");
        }

        [Test]
        public async Task CancelStarpoints_removes_starpoints_from_database()
        {
            throw new NotImplementedException("to do");
        }


        #endregion

    }
}
