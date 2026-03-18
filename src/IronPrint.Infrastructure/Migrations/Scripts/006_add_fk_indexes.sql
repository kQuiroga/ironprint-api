-- routine_days
CREATE INDEX ix_routine_days_routine_id ON routine_days (routine_id);

-- routine_exercises
CREATE INDEX ix_routine_exercises_routine_day_id ON routine_exercises (routine_day_id);
CREATE INDEX ix_routine_exercises_exercise_id    ON routine_exercises (exercise_id);

-- workout_sessions
CREATE INDEX ix_workout_sessions_routine_day_id ON workout_sessions (routine_day_id) WHERE routine_day_id IS NOT NULL;

-- exercise_logs
CREATE INDEX ix_exercise_logs_workout_session_id ON exercise_logs (workout_session_id);
CREATE INDEX ix_exercise_logs_exercise_id        ON exercise_logs (exercise_id);

-- set_logs
CREATE INDEX ix_set_logs_exercise_log_id ON set_logs (exercise_log_id);
