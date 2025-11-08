namespace TN3.Admin.Shared.Systems.Consts.ErrorCodes
{
    public static class CommonErrorCodes
    {
        public const string NotFound = "Common:0001";
        public const string EmailAlreadyExist = "Common:0003";
        public const string InvalidRequirement = "Common:0004";
        public const string InvalidTimeRange = "Common:0006";
        public const string CodeAlreadyExist = "Common:0008";
        public const string NotExecute = "Common:0009";
        public const string NameAlreadyExist = "Common:0010";
        public const string PhoneNumberAlreadyExist = "Common:0011";
        public const string TooLongContent = "Common:0013";
        public const string NotMatchId = "Common:0014";
        public const string ActionFailure = "Common:0015";
        public const string InvalidExtension = "Common:0016";
        public const string NotUpdateRoot = "Common:0017";
        public const string CannotDeleteBecauseReferenced = "Common:0018";
        public const string InvalidOperation = "Common:0019";
    }

    public static class UserErrorCodes
    {
        public const string WrongPassword = "User:0001";
        public const string UserAlreadyExist = "User:0002";
        public const string LoginFailure = "User:0003";
        public const string DeactivatedUser = "User:0004";
        public const string NotFound = "User:0005";
        public const string InvalidDob = "User:0007";
        public const string NameTooLong = "User:0008";
    }

    public class UploadErrorCodes
    {
        public const string InvalidExtension = "Upload:0001";
    }

    public class RoleErrorCodes
    {
        public const string NameAlreadyExist = "Role:0001";
        public const string NameTooLong = "Role:0002";
    }
    public class PatientErrorCodes
    {
        public const string CannotDeletePatientWithAppointments = "Patient:0001";
    }
    
    public class CustomerErrorCodes
    {
        public const string NotFound = "Customer:0001";
        public const string CannotDeleteCustomerWithAppointments = "Customer:0002";
        public const string PhoneNumberAlreadyExists = "Customer:0003";
        public const string EmailAlreadyExists = "Customer:0004";
    }
    
    public class DoctorErrorCodes
    {
        public const string CannotDeleteDoctorWithAppointments = "Doctor:0001";
        public const string ScheduleOverlap = "Doctor:0002";
        public const string InvalidWorkingHours = "Doctor:0003";
    }
      public class ServiceErrorCodes
    {
        public const string CannotDeleteServiceInUse = "Service:0001";
        public const string CannotDeleteServiceWithInvoiceDetails = "Service:0002";
    }
    
    public class AppointmentErrorCodes
    {
        public const string PatientNotFound = "Appointment:0001";
        public const string DoctorNotFound = "Appointment:0002";
        public const string ServiceNotFound = "Appointment:0003";
        public const string AppointmentTimeMustBeInFuture = "Appointment:0004";
        public const string DoctorHasConflictingAppointment = "Appointment:0005";
        public const string CannotUpdateCompletedOrCancelledAppointment = "Appointment:0006";
        public const string CannotDeleteConfirmedOrCompletedAppointment = "Appointment:0007";
        public const string CanOnlyConfirmPendingAppointment = "Appointment:0008";
        public const string CannotCancelCompletedOrAlreadyCancelledAppointment = "Appointment:0009";
        public const string CanOnlyCompleteConfirmedAppointment = "Appointment:0010";
    }
    
    public class InvoiceErrorCodes
    {
        public const string AppointmentNotFound = "Invoice:0001";
        public const string CanOnlyCreateInvoiceForCompletedAppointment = "Invoice:0002";
        public const string AppointmentAlreadyHasInvoice = "Invoice:0003";
        public const string PatientNotFound = "Invoice:0004";
        public const string MustHaveAtLeastOneService = "Invoice:0005";
        public const string ServiceNotFound = "Invoice:0006";
        public const string CannotUpdatePaidInvoice = "Invoice:0007";
        public const string CannotDeletePaidInvoice = "Invoice:0008";
        public const string InvoiceAlreadyPaid = "Invoice:0009";
        public const string PaymentAmountDoesNotMatch = "Invoice:0010";
        public const string CannotCancelPaidInvoice = "Invoice:0011";
        public const string InvoiceAlreadyExistsForAppointment = "Invoice:0012";
    }
    
    public class AppConfigErrorCodes
    {
        public const string FailedEmailConnection = "AppConfig:0001";
        public const string NotFound = "AppConfig:0002";
        public const string NotMutipleConfig = "AppConfig:0003";
    }
    
    public class PackageErrorCodes
    {
        public const string TenantUsing = "Package:0001";
    }
    
    public class StaffErrorCodes
    {
        public const string UserAlreadyLinkedToAnotherStaff = "Staff:0001";
    }
}