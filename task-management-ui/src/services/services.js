import api from './api';

export const authService = {
  register: (data) => api.post('/auth/register', data).then(r => r.data),
  login:    (data) => api.post('/auth/login',    data).then(r => r.data),
  profile:  ()     => api.get('/auth/profile').then(r => r.data),
};

export const taskService = {
  getAll:    (params) => api.get('/tasks', { params }).then(r => r.data),
  getById:   (id)     => api.get(`/tasks/${id}`).then(r => r.data),
  getCounts: ()       => api.get('/tasks/counts').then(r => r.data),
  create:    (data)   => api.post('/tasks', data).then(r => r.data),
  update:    (id, d)  => api.put(`/tasks/${id}`, d).then(r => r.data),
  remove:    (id)     => api.delete(`/tasks/${id}`).then(r => r.data),
};

export const categoryService = {
  getAll:  ()     => api.get('/categories').then(r => r.data),
  create:  (data) => api.post('/categories', data).then(r => r.data),
  remove:  (id)   => api.delete(`/categories/${id}`).then(r => r.data),
};

export const userService = {
  getAll:          ()     => api.get('/users').then(r => r.data),
  getById:         (id)   => api.get(`/users/${id}`).then(r => r.data),
  changePassword:  (data) => api.post('/users/change-password', data).then(r => r.data),
  changeRole:      (data) => api.post('/users/change-role', data).then(r => r.data),
  remove:          (id)   => api.delete(`/users/${id}`).then(r => r.data),
};
