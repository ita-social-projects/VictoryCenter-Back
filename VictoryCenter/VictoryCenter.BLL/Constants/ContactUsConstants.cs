using VictoryCenter.BLL.DTOs.Public.ContactUs;

namespace VictoryCenter.BLL.Constants;

public static class ContactUsConstants
{
    public static readonly int NameMaxLength = 256;

    public static readonly int NameMinLength = 1;

    public static readonly int EmailAddressMaxLength = 256;

    public static readonly int EmailSubjectMinLength = 5;

    public static readonly int EmailSubjectMaxLength = 100;

    public static readonly int EmailMessageMinLength = 10;

    public static readonly int EmailMessageMaxLength = 2000;

    public static string EmailSubjectTemplate(ContactUsFormDto dto)
    {
        return $"Нове заповнення контактної форми від {dto.FromName}. Тема: {dto.Subject}";
    }

    public static string EmailTextBodyTemplate(ContactUsFormDto dto)
    {
        return $"Імʼя відправника: {dto.FromName}\nТема: {dto.Subject}\nПовідомлення:\n{dto.Message}";
    }
}
