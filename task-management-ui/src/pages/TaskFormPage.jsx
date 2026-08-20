import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { taskService, categoryService, userService } from '../services/services';

const STATUSES   = ['Pending', 'InProgress', 'Completed'];
const PRIORITIES = ['Low', 'Medium', 'High'];

const empty = {
  title: '', description: '', priority: 'Medium',
  status: 'Pending', dueDate: '', categoryId: '', assignedToUserId: ''
};

export default function TaskFormPage() {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const { isAdmin } = useAuth();

  const [form,       setForm]       = useState(empty);
  const [categories, setCategories] = useState([]);
  const [users,      setUsers]      = useState([]);
  const [loading,    setLoading]    = useState(isEdit);
  const [saving,     setSaving]     = useState(false);
  const [error,      setError]      = useState('');

  // Load categories (all users) and users list (admin only)
  useEffect(() => {
    categoryService.getAll().then(setCategories).catch(console.error);
    if (isAdmin) userService.getAll().then(setUsers).catch(console.error);
  }, [isAdmin]);

  // Load existing task when editing
  useEffect(() => {
    if (!isEdit) return;
    taskService.getById(id)
      .then(t => setForm({
        title:            t.title,
        description:      t.description || '',
        priority:         t.priority,
        status:           t.status,
        dueDate:          t.dueDate ? t.dueDate.split('T')[0] : '',
        categoryId:       t.categoryId?.toString() || '',
        assignedToUserId: t.assignedToUserId || ''
      }))
      .catch(() => setError('Failed to load task.'))
      .finally(() => setLoading(false));
  }, [id, isEdit]);

  const handleChange = (e) => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setSaving(true);
    try {
      const payload = {
        ...form,
        categoryId:       form.categoryId ? Number(form.categoryId) : null,
        dueDate:          form.dueDate || null,
        assignedToUserId: form.assignedToUserId || null
      };
      if (isEdit) await taskService.update(id, payload);
      else        await taskService.create(payload);
      navigate('/tasks');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to save task.');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div className="spinner" />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">{isEdit ? `Edit Task #${id}` : 'Create New Task'}</h1>
          <p className="page-subtitle">{isEdit ? 'Update task details below' : 'Fill in the details to create a task'}</p>
        </div>
      </div>

      <div className="card" style={{ maxWidth: 680 }}>
        {error && <div className="alert alert-error" style={{ marginBottom: '1rem' }}>{error}</div>}

        <form onSubmit={handleSubmit}>
          {/* Title */}
          <div className="form-group">
            <label className="form-label">Title *</label>
            <input id="task-title" className="form-input" name="title"
              placeholder="Task title" value={form.title} onChange={handleChange} required />
          </div>

          {/* Description */}
          <div className="form-group">
            <label className="form-label">Description</label>
            <textarea id="task-desc" className="form-textarea" name="description"
              placeholder="Describe the task..." value={form.description} onChange={handleChange} />
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            {/* Priority */}
            <div className="form-group">
              <label className="form-label">Priority</label>
              <select id="task-priority" className="form-select" name="priority"
                value={form.priority} onChange={handleChange}>
                {PRIORITIES.map(p => <option key={p} value={p}>{p}</option>)}
              </select>
            </div>

            {/* Status (edit mode only) */}
            {isEdit && (
              <div className="form-group">
                <label className="form-label">Status</label>
                <select id="task-status" className="form-select" name="status"
                  value={form.status} onChange={handleChange}>
                  {STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
                </select>
              </div>
            )}
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
            {/* Due Date */}
            <div className="form-group">
              <label className="form-label">Due Date</label>
              <input id="task-due" className="form-input" type="date" name="dueDate"
                value={form.dueDate} onChange={handleChange} />
            </div>

            {/* Category Dropdown from API */}
            <div className="form-group">
              <label className="form-label">Category</label>
              <select id="task-category" className="form-select" name="categoryId"
                value={form.categoryId} onChange={handleChange}>
                <option value="">— No Category —</option>
                {categories.map(c => (
                  <option key={c.id} value={c.id}>{c.name}</option>
                ))}
              </select>
            </div>
          </div>

          {/* Assign To User (Admin only, loaded from API) */}
          {isAdmin && (
            <div className="form-group">
              <label className="form-label">Assign To User</label>
              <select id="task-assign" className="form-select" name="assignedToUserId"
                value={form.assignedToUserId} onChange={handleChange}>
                <option value="">— Assign to myself —</option>
                {users.map(u => (
                  <option key={u.id} value={u.id}>
                    {u.firstName} {u.lastName} ({u.email})
                  </option>
                ))}
              </select>
            </div>
          )}

          <div style={{ display: 'flex', gap: '1rem', marginTop: '0.75rem' }}>
            <button id="save-task-btn" className="btn btn-primary" type="submit" disabled={saving}>
              {saving ? 'Saving...' : isEdit ? '💾 Update Task' : '🚀 Create Task'}
            </button>
            <button type="button" className="btn btn-ghost" onClick={() => navigate('/tasks')}>
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
