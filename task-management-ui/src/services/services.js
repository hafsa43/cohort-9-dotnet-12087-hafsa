import api from './api';

export const authService = {
  register: (data) => api.post('/auth/register', data).then(r => r.data),
  login:    (data) => api.post('/auth/login',    data).then(r => r.data),
  profile:  ()     => api.get('/auth/profile').then(r => r.data),
};

export const taskService = {
  getAll:   ()       => api.get('/tasks').then(r => r.data),
  getById:  (id)     => api.get(`/tasks/${id}`).then(r => r.data),
  getCounts:()       => api.get('/tasks/counts').then(r => r.data),
  create:   (data)   => api.post('/tasks', data).then(r => r.data),
  update:   (id, d)  => api.put(`/tasks/${id}`, d).then(r => r.data),
  remove:   (id)     => api.delete(`/tasks/${id}`).then(r => r.data),
};
