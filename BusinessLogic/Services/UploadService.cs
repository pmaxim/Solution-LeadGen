using System;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorWeb.Shared.Models;
using Domain.Models;
using Domain.Repositories;

namespace BusinessLogic.Services
{
    public interface IUploadService
    {
        Task<int> UploadPhones(List<string> phones, string userName);
        Task<TablePhonesViewModel> GetTablePhones(string userName, string role);
        Task RemovePhones(string userName, bool isCall);
    }

    public class UploadService : IUploadService
    {
        private readonly ILeadPhoneRepository _leadPhoneRepository;
        private readonly IMapper _mapper;

        public UploadService(ILeadPhoneRepository leadPhoneRepository,
            IMapper mapper)
        {
            _leadPhoneRepository = leadPhoneRepository;
            _mapper = mapper;
        }

        public async Task<int> UploadPhones(List<string> phones, string userName)
        {
            return await _leadPhoneRepository.UploadPhones(phones, userName);
        }

        public async Task<TablePhonesViewModel> GetTablePhones(string userName, string role)
        {
            var model = new TablePhonesViewModel();
            var item = new TablePhoneItem
            {
                CountIsCall = await _leadPhoneRepository.CountPhones(userName, role, true),
                CountNoCall = await _leadPhoneRepository.CountPhones(userName, role, false),
                Title = role == Constants.RoleAdmin ? "All Users" : userName
            };
            
            model.List.Add(item);

            if (role == Constants.RoleAdmin)
            {
                var itemAdmin = new TablePhoneItem
                {
                    CountIsCall = await _leadPhoneRepository.CountPhones(userName, string.Empty, true),
                    CountNoCall = await _leadPhoneRepository.CountPhones(userName, string.Empty, false),
                    Title = userName
                };

                model.List.Add(itemAdmin);
            }

            return model;
        }

        public async Task RemovePhones(string userName, bool isCall)
        {
            await _leadPhoneRepository.RemovePhonesSql(userName, isCall);
        }
    }
}
