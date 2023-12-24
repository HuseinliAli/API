﻿using Business.Messages;
using Client.Utilities.Constants;
using Client.Utilities.FileHelpers;
using Entities.Concrete;
using Entities.Dtos.Centers;
using Entities.Dtos.Stadiums;
using Framework.Entities;
using Framework.Utilities.Results;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Clients
{
    public class StadiumClient
    {
        public async Task<List<Stadium>> GetByUser()
        {
            using (var client = new HttpClient())
            {
                var token = TokenFileHelper.ReadTokenFromJsonFile(Urls.TokenFilePath);
                var response = await client.GetAsync($"{StadiumUrls.ByUser}{FindId(token)}");
                if(response.IsSuccessStatusCode)
                {
                    var result =await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Stadium>>(result);
                }
                return new();
            }
        }
        public async Task<IResult> AddStadium(AddOrUpdateStadiumDto dto)
        {
            using (var client = new HttpClient())
            {
                var token = TokenFileHelper.ReadTokenFromJsonFile(Urls.TokenFilePath);
                dto.UserId = FindId(token);
                var json = JsonConvert.SerializeObject(dto);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ token);
                var response = await client.PostAsync($"{StadiumUrls.BaseUrl}", new StringContent(json, Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<SuccessResult>(result);
                }
                return new ErrorResult(StadiumResultMessage.AddFailed);
            }
        }
        private int FindId(string token)
        {
            var decodedToken = new JwtSecurityToken(token);
            string nameIdentifier = decodedToken?.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return int.Parse(nameIdentifier);
        }
    }
}
