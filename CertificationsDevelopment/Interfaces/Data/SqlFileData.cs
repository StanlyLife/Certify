using CertificationsDevelopment.Data;
using CertificationsDevelopment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificationsDevelopment.Interfaces.Data {
	public class SqlFileData : IFileUploadData {
		private readonly ApplicationDbContext db;

		public SqlFileData(ApplicationDbContext db) {
			this.db = db;
		}
		public Fileupload Add(Fileupload file) {
			db.Add(file);
			return file;
		}

		public int Commit() {
			return db.SaveChanges();
		}
		public IEnumerable<Fileupload> GetByCertId(int id) {
			var query = from file in db.Files
						where file.CertificationId == id
						select file;
			return query;
		}

		public Fileupload Delete(int CertificationId) {
			var _file = GetByCertId(CertificationId);
			if(_file != null) {
				foreach(var x in _file) {
					db.Files.Remove(x);
				}
			}
			return _file.FirstOrDefault();
		}


		public Fileupload GetByFileId(int id) {
			return db.Files.Find(id);
		}

		public Fileupload Update(Fileupload file) {
			var entity = db.Files.Attach(file);
			entity.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			return file;
		}

		public bool DoesFileExist(Fileupload file) {
			var query = from data in db.Files
						where data.FileData == file.FileData
						select data;
			if (!query.Any()) {
				return false;
			}
			return true;
		}
	}
}
