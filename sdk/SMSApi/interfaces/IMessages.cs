﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InteractuaMovil.ContactoSms.Api.interfaces
{

    public interface IMessages
    {
        ResponseObjects.ApiResponse<List<ResponseObjects.MessageResponse>> GetList(DateTime? StartDate = null, DateTime? EndDate = null, Int32 Start = -1, Int32 Limit = -1, String Msisdn = null, String ShortName = null, Boolean IncludeRecipients = false, MessageDirection Direction = MessageDirection.MT, String Username = null );
        ResponseObjects.ApiResponse<ResponseObjects.MessageResponse> SendToGroups(String[] ShortName, String Message, String Id = null);
        ResponseObjects.ApiResponse<ResponseObjects.MessageResponse> SendToContact(String Msisdn, String Message, String Id = null);
        ResponseObjects.ApiResponse<List<ResponseObjects.ScheduleMessageResponse>> GetSchedule();
        ResponseObjects.ApiResponse<ResponseObjects.ActionMessageResponse> RemoveSchedule(String MessageId);
        ResponseObjects.ApiResponse<ResponseObjects.ActionMessageResponse> AddSchedule(DateTime StartDate, DateTime EndDate, String Name, String Message, String Time, String Frequency, String[] Groups);
        //ResponseObjects.ApiResponse<List<ResponseObjects.InboxMessageResponse>> Inbox(DateTime? StartDate = null, DateTime? EndDate = null, int Start = -1, int Limit = -1, string Msisdn = null, int Status = -1);
    }
}
