using Dapper;
using Lar.Domain.Dto;
using Lar.Domain.Entities;
using Lar.Domain.Interface.Repositories;
using System.Data;

namespace Lar.Infra.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly IDbConnection _dbConnection;

        public PersonRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Person> RegisterPerson(PersonDto person)
        {
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string insertQuery = @"
                        INSERT INTO Pessoa (Name, CPF, DateBirth, Active)
                        VALUES (@Name, @CPF, @DateBirth, @Active);
                        SELECT LAST_INSERT_ID();";

                    var personId = await _dbConnection.ExecuteScalarAsync<int>(insertQuery, person, transaction);

                    if (person.Telephones != null && person.Telephones.Any())
                    {
                        foreach (var phone in person.Telephones)
                        {
                            string insertPhoneQuery = @"
                                INSERT INTO Telefones (PessoaId, Numero, Tipo)
                                VALUES (@PessoaId, @Numero, @Tipo);";

                            await _dbConnection.ExecuteAsync(insertPhoneQuery, new { PessoaId = personId, phone.Number, phone.Tipo }, transaction);
                        }
                    }

                    string selectPersonQuery = "SELECT * FROM Pessoa WHERE Id = @Id";
                    var insertedPerson = await _dbConnection.QueryFirstOrDefaultAsync<Person>(selectPersonQuery, new { Id = personId }, transaction);

                    transaction.Commit();
                    return insertedPerson;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<Person> GetPerson(int id)
        {
            string selectQuery = "SELECT * FROM Pessoa WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Person>(selectQuery, new { Id = id });
        }

        public async Task<Person> UpdatePerson(int id, PersonDto person)
        {
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string selectQuery = "SELECT COUNT(*) FROM Pessoa WHERE Id = @Id";
                    int count = await _dbConnection.ExecuteScalarAsync<int>(selectQuery, new { Id = id }, transaction);
                    if (count == 0)
                    {
                        throw new Exception("The person could not be reached to update their registration");
                    }

                    string updateQuery = @"
                UPDATE Pessoa
                SET Name = @Name, CPF = @CPF, DateBirth = @DateBirth, Active = @Active
                WHERE Id = @Id";

                    await _dbConnection.ExecuteAsync(updateQuery, new { Id = id, person.Name, person.CPF, person.DateBirth, person.Active }, transaction);

                    string deletePhonesQuery = "DELETE FROM Telefones WHERE PessoaId = @PessoaId";
                    await _dbConnection.ExecuteAsync(deletePhonesQuery, new { PessoaId = id }, transaction);

                    if (person.Telephones != null && person.Telephones.Any())
                    {
                        foreach (var phone in person.Telephones)
                        {
                            string insertPhoneQuery = @"
                        INSERT INTO Telefones (PessoaId, Numero, Tipo)
                        VALUES (@PessoaId, @Numero, @Tipo);";

                            await _dbConnection.ExecuteAsync(insertPhoneQuery, new { PessoaId = id, phone.Number, phone.Tipo }, transaction);
                        }
                    }

                    string selectPersonQuery = "SELECT * FROM Pessoa WHERE Id = @Id";
                    var updatedPerson = await _dbConnection.QueryFirstOrDefaultAsync<Person>(selectPersonQuery, new { Id = id }, transaction);

                    transaction.Commit();
                    return updatedPerson;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public async Task<bool> DeletePerson(int id)
        {
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    string deletePersonQuery = "DELETE FROM Pessoa WHERE Id = @Id";
                    int rowsAffected = await _dbConnection.ExecuteAsync(deletePersonQuery, new { Id = id }, transaction);

                    bool success = rowsAffected > 0;

                    transaction.Commit();
                    return success;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

        }
    }
}
