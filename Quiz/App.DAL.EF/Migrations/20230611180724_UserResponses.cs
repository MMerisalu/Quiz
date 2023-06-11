using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class UserResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserResponse_Answers_AnswerId",
                table: "UserResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserResponse_Questions_QuestionId",
                table: "UserResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserResponse_Quizzes_QuizId",
                table: "UserResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserResponse",
                table: "UserResponse");

            migrationBuilder.RenameTable(
                name: "UserResponse",
                newName: "UserResponses");

            migrationBuilder.RenameIndex(
                name: "IX_UserResponse_QuizId",
                table: "UserResponses",
                newName: "IX_UserResponses_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_UserResponse_QuestionId",
                table: "UserResponses",
                newName: "IX_UserResponses_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserResponse_AnswerId",
                table: "UserResponses",
                newName: "IX_UserResponses_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserResponses",
                table: "UserResponses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponses_Answers_AnswerId",
                table: "UserResponses",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponses_Questions_QuestionId",
                table: "UserResponses",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponses_Quizzes_QuizId",
                table: "UserResponses",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserResponses_Answers_AnswerId",
                table: "UserResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserResponses_Questions_QuestionId",
                table: "UserResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserResponses_Quizzes_QuizId",
                table: "UserResponses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserResponses",
                table: "UserResponses");

            migrationBuilder.RenameTable(
                name: "UserResponses",
                newName: "UserResponse");

            migrationBuilder.RenameIndex(
                name: "IX_UserResponses_QuizId",
                table: "UserResponse",
                newName: "IX_UserResponse_QuizId");

            migrationBuilder.RenameIndex(
                name: "IX_UserResponses_QuestionId",
                table: "UserResponse",
                newName: "IX_UserResponse_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserResponses_AnswerId",
                table: "UserResponse",
                newName: "IX_UserResponse_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserResponse",
                table: "UserResponse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponse_Answers_AnswerId",
                table: "UserResponse",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponse_Questions_QuestionId",
                table: "UserResponse",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserResponse_Quizzes_QuizId",
                table: "UserResponse",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
