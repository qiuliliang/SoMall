﻿using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace TT.Abp.Mall.Domain.Users
{
    public class MallUserSynchronizer :
        IDistributedEventHandler<EntityUpdatedEto<UserEto>>,
        ITransientDependency
    {
        public MallUserSynchronizer(
            IMallUserRepository userRepository,
            IMallUserLookupService userLookupService)
        {
            UserRepository = userRepository;
            UserLookupService = userLookupService;
        }

        protected IMallUserRepository UserRepository { get; }
        protected IMallUserLookupService UserLookupService { get; }

        public async Task HandleEventAsync(EntityUpdatedEto<UserEto> eventData)
        {
            var user = await UserRepository.FindAsync(eventData.Entity.Id);
            if (user == null)
            {
                user = await UserLookupService.FindByIdAsync(eventData.Entity.Id);
                if (user == null)
                {
                    return;
                }
            }

            if (user.Update(eventData.Entity))
            {
                await UserRepository.UpdateAsync(user);
            }
        }
    }
}