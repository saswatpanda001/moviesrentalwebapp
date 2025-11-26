using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesRental.Migrations
{
    /// <inheritdoc />
    public partial class validations1_new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReleaseYear",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Movies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Email_MinLength",
                table: "Users",
                sql: "LEN([Email]) >= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Name_MinLength",
                table: "Users",
                sql: "LEN([Name]) >= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Phone_10Digits",
                table: "Users",
                sql: "LEN([Phone]) = 10 AND [Phone] NOT LIKE '%[^0-9]%'");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Role_Enum",
                table: "Users",
                sql: "[Role] IN ('User','Admin')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Comment_MinLength",
                table: "Reviews",
                sql: "LEN([Comment]) >= 5 OR [Comment] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reviews_Rating_Range",
                table: "Reviews",
                sql: "[Rating] >= 1 AND [Rating] <= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rentals_DueDate_After_RentedOn",
                table: "Rentals",
                sql: "[DueDate] > [RentedOn]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rentals_Price_Positive",
                table: "Rentals",
                sql: "[Price] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Rentals_ReturnedOn_After_RentedOn",
                table: "Rentals",
                sql: "[ReturnedOn] IS NULL OR [ReturnedOn] >= [RentedOn]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Amount_Positive",
                table: "Payments",
                sql: "[Amount] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_PaymentMethod_MinLength",
                table: "Payments",
                sql: "LEN([PaymentMethod]) >= 3");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Status_MinLength",
                table: "Payments",
                sql: "LEN([Status]) >= 3");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movies_Desc_MinLength",
                table: "Movies",
                sql: "LEN([Description]) >= 6");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movies_Genre_MinLength",
                table: "Movies",
                sql: "LEN([Genre]) >= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movies_ReleaseYear_Range",
                table: "Movies",
                sql: "[ReleaseYear] >= 1801 AND [ReleaseYear] <= 2025");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movies_RentalPrice_Positive",
                table: "Movies",
                sql: "[RentalPrice] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movies_Stock_NonNegative",
                table: "Movies",
                sql: "[Stock] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movies_Title_MinLength",
                table: "Movies",
                sql: "LEN([Title]) >= 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CartItem_Quantity_Range",
                table: "CartItems",
                sql: "[Quantity] >= 1 AND [Quantity] <= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CartItem_RentalDays_Range",
                table: "CartItems",
                sql: "[RentalDays] >= 1 AND [RentalDays] <= 30");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Email_MinLength",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Name_MinLength",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Phone_10Digits",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Role_Enum",
                table: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Comment_MinLength",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reviews_Rating_Range",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Rentals_DueDate_After_RentedOn",
                table: "Rentals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Rentals_Price_Positive",
                table: "Rentals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Rentals_ReturnedOn_After_RentedOn",
                table: "Rentals");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Amount_Positive",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_PaymentMethod_MinLength",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Status_MinLength",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movies_Desc_MinLength",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movies_Genre_MinLength",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movies_ReleaseYear_Range",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movies_RentalPrice_Positive",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movies_Stock_NonNegative",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movies_Title_MinLength",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CartItem_Quantity_Range",
                table: "CartItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CartItem_RentalDays_Range",
                table: "CartItems");

            migrationBuilder.AlterColumn<int>(
                name: "ReleaseYear",
                table: "Movies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "Movies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
