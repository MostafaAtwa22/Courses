export enum Module {
  Role = 'Role',
  User = 'User',
  Admin = 'Admin',
  Student = 'Student',
  Instructor = 'Instructor',
  Category = 'Category'
}

export enum CRUD {
  Create = 'Create',
  Read = 'Read',
  Update = 'Update',
  Delete = 'Delete'
}

export function buildPermission(module: Module, action: CRUD): string {
  return `Permission:${module}:${action}`;
}

export const PERMISSIONS = {
  ROLE: {
    READ: buildPermission(Module.Role, CRUD.Read),
    CREATE: buildPermission(Module.Role, CRUD.Create),
    UPDATE: buildPermission(Module.Role, CRUD.Update),
    DELETE: buildPermission(Module.Role, CRUD.Delete),
  },
  USER: {
    READ: buildPermission(Module.User, CRUD.Read),
    CREATE: buildPermission(Module.User, CRUD.Create),
    UPDATE: buildPermission(Module.User, CRUD.Update),
    DELETE: buildPermission(Module.User, CRUD.Delete),
  },
  ADMIN: {
    READ: buildPermission(Module.Admin, CRUD.Read),
    CREATE: buildPermission(Module.Admin, CRUD.Create),
    UPDATE: buildPermission(Module.Admin, CRUD.Update),
    DELETE: buildPermission(Module.Admin, CRUD.Delete),
  },
  STUDENT: {
    READ: buildPermission(Module.Student, CRUD.Read),
    CREATE: buildPermission(Module.Student, CRUD.Create),
    UPDATE: buildPermission(Module.Student, CRUD.Update),
    DELETE: buildPermission(Module.Student, CRUD.Delete),
  },
  INSTRUCTOR: {
    READ: buildPermission(Module.Instructor, CRUD.Read),
    CREATE: buildPermission(Module.Instructor, CRUD.Create),
    UPDATE: buildPermission(Module.Instructor, CRUD.Update),
    DELETE: buildPermission(Module.Instructor, CRUD.Delete),
  },
  CATEGORY: {
    READ: buildPermission(Module.Category, CRUD.Read),
    CREATE: buildPermission(Module.Category, CRUD.Create),
    UPDATE: buildPermission(Module.Category, CRUD.Update),
    DELETE: buildPermission(Module.Category, CRUD.Delete),
  },
};
